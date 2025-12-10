using System.Collections.Concurrent;
using System.Diagnostics;
using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

/// <summary>
/// Handler for uploading multiple files to S3 with real-time progress tracking
/// Uses AWS TransferUtility for efficient uploads and SignalR for progress broadcasting
/// </summary>
public sealed class UploadMultipleFilesCommandHandler
    : ICommandHandler<Command.UploadMultipleFilesCommand, Response.UploadMultipleFilesResponseDto>
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IUploadProgressService _uploadProgressService;
    private readonly ILogger<UploadMultipleFilesCommandHandler> _logger;

    private const long MaxFileSizeInBytes = 100 * 1024 * 1024; // 100MB per file
    private const int MaxConcurrentUploads = 3; // Upload 3 files concurrently

    public UploadMultipleFilesCommandHandler(
        IFileStorageService fileStorageService,
        IUploadProgressService uploadProgressService,
        ILogger<UploadMultipleFilesCommandHandler> logger)
    {
        _fileStorageService = fileStorageService ?? throw new ArgumentNullException(nameof(fileStorageService));
        _uploadProgressService = uploadProgressService ?? throw new ArgumentNullException(nameof(uploadProgressService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<Result<Response.UploadMultipleFilesResponseDto>> Handle(
        Command.UploadMultipleFilesCommand request,
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        var startedAt = DateTime.UtcNow;

        _logger.LogInformation(
            "Starting multiple file upload. UploadingProgressId: {ProgressId}, AppServiceName: {AppServiceName}, FilesCount: {Count}",
            request.UploadingProgressId, request.AppServiceName, request.Files?.Count ?? 0);

        // Business validation
        var validationResult = ValidateRequest(request);
        if (validationResult.IsFailure)
        {
            return Result<Response.UploadMultipleFilesResponseDto>.Failure(validationResult.Error);
        }

        // Thread-safe collection for upload results
        var results = new ConcurrentBag<Response.FileUploadResultDto>();
        var semaphore = new SemaphoreSlim(MaxConcurrentUploads, MaxConcurrentUploads);

        // Create upload tasks for all files
        var uploadTasks = request.Files.Select(async (file, index) =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var result = await UploadSingleFileAsync(
                    file,
                    request.AppServiceName,
                    request.UploadingProgressId,
                    index,
                    request.Files.Count,
                    cancellationToken);

                results.Add(result);
            }
            finally
            {
                semaphore.Release();
            }
        });

        // Wait for all uploads to complete
        await Task.WhenAll(uploadTasks);

        stopwatch.Stop();
        var completedAt = DateTime.UtcNow;

        // Calculate statistics
        var resultsList = results.ToList();
        var successCount = resultsList.Count(r => r.IsSuccess);
        var failedCount = resultsList.Count(r => !r.IsSuccess);

        // Report completion via SignalR
        await _uploadProgressService.ReportCompletionAsync(
            request.UploadingProgressId,
            successCount,
            failedCount,
            cancellationToken);

        var response = new Response.UploadMultipleFilesResponseDto
        {
            UploadingProgressId = request.UploadingProgressId,
            TotalFiles = request.Files.Count,
            SuccessfulUploads = successCount,
            FailedUploads = failedCount,
            Results = resultsList.OrderBy(r => r.FileName).ToList(),
            StartedAt = startedAt,
            CompletedAt = completedAt,
            TotalDurationSeconds = stopwatch.Elapsed.TotalSeconds
        };

        _logger.LogInformation(
            "Multiple file upload completed. UploadingProgressId: {ProgressId}, Success: {Success}, Failed: {Failed}, Duration: {Duration}s",
            request.UploadingProgressId, successCount, failedCount, response.TotalDurationSeconds);

        // Return success even if some files failed (partial success)
        return Result<Response.UploadMultipleFilesResponseDto>.Success(response);
    }

    /// <summary>
    /// Uploads a single file with progress tracking
    /// </summary>
    private async Task<Response.FileUploadResultDto> UploadSingleFileAsync(
        Microsoft.AspNetCore.Http.IFormFile file,
        string appServiceName,
        string uploadingProgressId,
        int fileIndex,
        int totalFiles,
        CancellationToken cancellationToken)
    {
        var result = new Response.FileUploadResultDto
        {
            FileName = file.FileName,
            FileSize = file.Length
        };

        _logger.LogInformation(
            "Uploading file {Index}/{Total}: {FileName} ({Size} bytes)",
            fileIndex + 1, totalFiles, file.FileName, file.Length);

        // Business validation for individual file
        var validationError = ValidateFile(file);
        if (validationError != null)
        {
            result.IsSuccess = false;
            result.ErrorMessage = validationError;

            await ReportProgressAsync(
                uploadingProgressId, file.FileName, fileIndex, totalFiles,
                0, file.Length, "failed", cancellationToken);

            return result;
        }

        try
        {
            // Generate object name
            var objectName = _fileStorageService.GenerateObjectName(
                appServiceName,
                file.FileName,
                uploadingProgressId);

            result.ObjectName = objectName;

            // Determine content type
            var contentType = _fileStorageService.GetContentType(file.FileName);
            result.ContentType = contentType;

            // Upload with progress tracking
            var objectUrl = await _fileStorageService.UploadFileWithProgressAsync(
                file,
                objectName,
                contentType,
                (transferred, total) =>
                {
                    // Report progress via SignalR (non-blocking)
                    _ = ReportProgressAsync(
                        uploadingProgressId,
                        file.FileName,
                        fileIndex,
                        totalFiles,
                        transferred,
                        total,
                        "uploading",
                        cancellationToken);
                },
                cancellationToken);

            // Generate download URL
            result.DownloadUrl = await _fileStorageService.GeneratePresignedDownloadUrlAsync(
                objectName,
                expiresInMinutes: 10080, // 7 days
                cancellationToken);

            result.IsSuccess = true;

            // Report completion for this file
            await ReportProgressAsync(
                uploadingProgressId, file.FileName, fileIndex, totalFiles,
                file.Length, file.Length, "completed", cancellationToken);

            _logger.LogInformation(
                "Successfully uploaded file {Index}/{Total}: {FileName} -> {ObjectName}",
                fileIndex + 1, totalFiles, file.FileName, objectName);
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;

            _logger.LogError(ex,
                "Failed to upload file {Index}/{Total}: {FileName}",
                fileIndex + 1, totalFiles, file.FileName);

            await ReportProgressAsync(
                uploadingProgressId, file.FileName, fileIndex, totalFiles,
                0, file.Length, "failed", cancellationToken);
        }

        return result;
    }

    /// <summary>
    /// Reports upload progress to connected clients via SignalR
    /// </summary>
    private async Task ReportProgressAsync(
        string uploadingProgressId,
        string fileName,
        int fileIndex,
        int totalFiles,
        long transferredBytes,
        long totalBytes,
        string status,
        CancellationToken cancellationToken)
    {
        try
        {
            var percentComplete = totalBytes > 0
                ? Math.Round((double)transferredBytes / totalBytes * 100, 2)
                : 0;

            var progress = new Response.UploadProgressDto
            {
                UploadingProgressId = uploadingProgressId,
                FileName = fileName,
                FileIndex = fileIndex,
                TotalFiles = totalFiles,
                TransferredBytes = transferredBytes,
                TotalBytes = totalBytes,
                PercentComplete = percentComplete,
                Status = status
            };

            await _uploadProgressService.ReportProgressAsync(progress, cancellationToken);
        }
        catch (Exception ex)
        {
            // Don't fail the upload if progress reporting fails
            _logger.LogWarning(ex, "Failed to report progress for file: {FileName}", fileName);
        }
    }

    /// <summary>
    /// Validates the upload request
    /// </summary>
    private Result ValidateRequest(Command.UploadMultipleFilesCommand request)
    {
        if (request.Files == null || request.Files.Count == 0)
        {
            _logger.LogWarning("Validation failed: No files provided");
            return Result.Failure(
                new Error("FileStorage.NoFiles", "At least one file must be provided"));
        }

        if (request.Files.Count > 20) // Limit to 20 files per request
        {
            _logger.LogWarning("Validation failed: Too many files ({Count})", request.Files.Count);
            return Result.Failure(
                new Error("FileStorage.TooManyFiles", "Maximum 20 files allowed per upload"));
        }

        if (string.IsNullOrWhiteSpace(request.UploadingProgressId))
        {
            _logger.LogWarning("Validation failed: UploadingProgressId is required");
            return Result.Failure(
                new Error("FileStorage.InvalidProgressId", "UploadingProgressId is required"));
        }

        return Result.Success();
    }

    /// <summary>
    /// Validates a single file
    /// </summary>
    private string? ValidateFile(Microsoft.AspNetCore.Http.IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return "File is empty";
        }

        if (file.Length > MaxFileSizeInBytes)
        {
            return $"File size exceeds maximum allowed size of {MaxFileSizeInBytes / 1024 / 1024}MB";
        }

        if (string.IsNullOrWhiteSpace(file.FileName))
        {
            return "File name is required";
        }

        return null; // Valid
    }
}
