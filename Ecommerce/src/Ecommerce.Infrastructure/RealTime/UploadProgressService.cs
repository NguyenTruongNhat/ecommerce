using Ecommerce.Application.Abstractions;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Infrastructure.RealTime;

/// <summary>
/// Console-based implementation of upload progress logging
/// </summary>
public sealed class UploadProgressService : IUploadProgressService
{
    private readonly ILogger<UploadProgressService> _logger;

    public UploadProgressService(ILogger<UploadProgressService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void LogProgress(
        string uploadingProgressId,
        string fileName,
        int fileIndex,
        int totalFiles,
        long transferredBytes,
        long totalBytes,
        string status)
    {
        var percentComplete = totalBytes > 0
            ? Math.Round((double)transferredBytes / totalBytes * 100, 2)
            : 0;

        _logger.LogInformation(
            "[Upload Progress] Session: {UploadingProgressId} | File: {FileName} ({FileIndex}/{TotalFiles}) | Progress: {PercentComplete}% ({TransferredBytes}/{TotalBytes} bytes) | Status: {Status}",
            uploadingProgressId, fileName, fileIndex + 1, totalFiles, percentComplete, transferredBytes, totalBytes, status);

        // Also log to console for immediate visibility
        Console.WriteLine(
            $"[{DateTime.Now:HH:mm:ss}] Upload Progress: {fileName} ({fileIndex + 1}/{totalFiles}) - {percentComplete}% - {status}");
    }

    public void LogCompletion(
        string uploadingProgressId,
        int successCount,
        int failedCount)
    {
        _logger.LogInformation(
            "[Upload Completed] Session: {UploadingProgressId} | Success: {SuccessCount} | Failed: {FailedCount}",
            uploadingProgressId, successCount, failedCount);

        Console.WriteLine(
            $"[{DateTime.Now:HH:mm:ss}] Upload Session Completed: {successCount} succeeded, {failedCount} failed");
    }
}
