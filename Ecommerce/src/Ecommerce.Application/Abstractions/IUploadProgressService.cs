using Ecommerce.Contract.Services.V1.FileStorage;

namespace Ecommerce.Application.Abstractions;

/// <summary>
/// Service for logging upload progress to console
/// </summary>
public interface IUploadProgressService
{
    /// <summary>
    /// Logs upload progress to console
    /// </summary>
    void LogProgress(string uploadingProgressId, string fileName, int fileIndex, int totalFiles, long transferredBytes, long totalBytes, string status);

    /// <summary>
    /// Logs completion of an upload session
    /// </summary>
    void LogCompletion(string uploadingProgressId, int successCount, int failedCount);
}
