using Ecommerce.Contract.Services.V1.FileStorage;

namespace Ecommerce.Application.Abstractions;

/// <summary>
/// Service for broadcasting upload progress to clients
/// </summary>
public interface IUploadProgressService
{
    /// <summary>
    /// Reports upload progress for a specific file
    /// </summary>
    Task ReportProgressAsync(Response.UploadProgressDto progress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reports completion of an upload session
    /// </summary>
    Task ReportCompletionAsync(string uploadingProgressId, int successCount, int failedCount, CancellationToken cancellationToken = default);
}
