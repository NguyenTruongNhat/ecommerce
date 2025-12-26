using System.Collections.Concurrent;
using System.Diagnostics;
using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Abstractions.Message;
using Ecommerce.Contract.Abstractions.Shared;
using Ecommerce.Contract.Services.V1.FileStorage;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Application.UserCases.V1.Commands.FileStorage;

/// <summary>
/// Handler for uploading multiple files to S3 with console progress logging
/// Uses AWS TransferUtility for efficient uploads
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

        try
        {
            _logger.LogInformation(
                "Starting multiple file upload. UploadingProgressId: {ProgressId}, AppServiceName: {AppServiceName}, FilesCount: {Count}",
                request.UploadingProgressId, request.AppServiceName, request.Files?.Count ?? 0);

            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  Multiple File Upload Started                                ║");
            Console.WriteLine($"╠══════════════════════════════════════════════════════════════╣");
            Console.WriteLine($"║  Session ID: {request.UploadingProgressId,-42} ║");
            Console.WriteLine($"║  Service: {request.AppServiceName,-49} ║");
            Console.WriteLine($"║  Files: {request.Files?.Count ?? 0,-53} ║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝\n");


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

            // Log completion
            _uploadProgressService.LogCompletion(
                request.UploadingProgressId,
                successCount,
                failedCount);

            Console.WriteLine($"\n╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine($"║  Upload Session Completed                                    ║");
            Console.WriteLine($"╠══════════════════════════════════════════════════════════════╣");
            Console.WriteLine($"║  Total Files: {request.Files.Count,-47} ║");
            Console.WriteLine($"║  Successful: {successCount,-48} ║");
            Console.WriteLine($"║  Failed: {failedCount,-52} ║");
            Console.WriteLine($"║  Duration: {stopwatch.Elapsed.TotalSeconds:F2}s{new string(' ', 47)} ║");
            Console.WriteLine($"╚══════════════════════════════════════════════════════════════╝\n");

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

            return Result<Response.UploadMultipleFilesResponseDto>.Success(response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex,
                "Fatal error during multiple file upload. UploadingProgressId: {ProgressId}",
                request.UploadingProgressId);

            Console.WriteLine($"\n❌ FATAL ERROR: {ex.Message}\n");

            return (Result<Response.UploadMultipleFilesResponseDto>)Result<Response.UploadMultipleFilesResponseDto>.Failure(
                new Error(
                    "FileStorage.UploadMultipleFailed",
                    $"Failed to upload files: {ex.Message}"));
        }
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

        try
        {
            _logger.LogInformation("Uploading file {Index}/{Total}: {FileName} ({Size} bytes)", fileIndex + 1, totalFiles, file.FileName, file.Length);

            // Generate object name
            var objectName = _fileStorageService.GenerateObjectName(
                appServiceName,
                file.FileName,
                uploadingProgressId);

            result.ObjectName = objectName;

            // Determine content type
            var contentType = _fileStorageService.GetContentType(file.FileName);
            result.ContentType = contentType;

            Console.WriteLine($"📤 Uploading {fileIndex + 1}/{totalFiles}: {file.FileName} ({file.Length:N0} bytes)");

            // Upload with progress tracking
            var objectUrl = await _fileStorageService.UploadFileWithProgressAsync(
                file,
                objectName,
                contentType,
                (transferred, total) =>
                {
                    // Log progress to console
                    _uploadProgressService.LogProgress(
                        uploadingProgressId,
                        file.FileName,
                        fileIndex,
                        totalFiles,
                        transferred,
                        total,
                        "uploading");
                },
                cancellationToken);

            // Generate download URL
            result.DownloadUrl = await _fileStorageService.GeneratePresignedDownloadUrlAsync(
                objectName,
                expiresInMinutes: 10080, // 7 days
                cancellationToken);

            result.IsSuccess = true;

            // Log completion
            _uploadProgressService.LogProgress(
                uploadingProgressId, file.FileName, fileIndex, totalFiles,
                file.Length, file.Length, "completed");

            Console.WriteLine($"✅ File {fileIndex + 1}/{totalFiles}: {file.FileName} - COMPLETED");

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

            _uploadProgressService.LogProgress(
                uploadingProgressId, file.FileName, fileIndex, totalFiles,
                0, file.Length, "failed");

            Console.WriteLine($"❌ File {fileIndex + 1}/{totalFiles}: {file.FileName} - FAILED: {ex.Message}");
        }

        return result;
    }

}
