using Ecommerce.Application.Abstractions;
using Ecommerce.Contract.Services.V1.FileStorage;
using Ecommerce.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Ecommerce.Infrastructure.RealTime;

/// <summary>
/// SignalR-based implementation of upload progress reporting
/// </summary>
public sealed class UploadProgressService : IUploadProgressService
{
    private readonly IHubContext<UploadProgressHub> _hubContext;
    private readonly ILogger<UploadProgressService> _logger;

    public UploadProgressService(
        IHubContext<UploadProgressHub> hubContext,
        ILogger<UploadProgressService> logger)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task ReportProgressAsync(
        Response.UploadProgressDto progress,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients
                .Group(progress.UploadingProgressId)
                .SendAsync("UploadProgress", progress, cancellationToken);

            _logger.LogDebug(
                "Progress reported for {UploadingProgressId}: {FileName} - {Percent}%",
                progress.UploadingProgressId, progress.FileName, progress.PercentComplete);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to report progress for {UploadingProgressId}",
                progress.UploadingProgressId);
        }
    }

    public async Task ReportCompletionAsync(
        string uploadingProgressId,
        int successCount,
        int failedCount,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await _hubContext.Clients
                .Group(uploadingProgressId)
                .SendAsync("UploadCompleted", new
                {
                    UploadingProgressId = uploadingProgressId,
                    SuccessCount = successCount,
                    FailedCount = failedCount,
                    CompletedAt = DateTime.UtcNow
                }, cancellationToken);

            _logger.LogInformation(
                "Upload completion reported for {UploadingProgressId}: {Success} succeeded, {Failed} failed",
                uploadingProgressId, successCount, failedCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to report completion for {UploadingProgressId}",
                uploadingProgressId);
        }
    }
}
