using Microsoft.AspNetCore.SignalR;

namespace Ecommerce.Presentation.Hubs;

/// <summary>
/// SignalR Hub for broadcasting file upload progress to connected clients
/// </summary>
public sealed class UploadProgressHub : Hub
{
    public async Task JoinUploadGroup(string uploadingProgressId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, uploadingProgressId);
    }

    public async Task LeaveUploadGroup(string uploadingProgressId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, uploadingProgressId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
