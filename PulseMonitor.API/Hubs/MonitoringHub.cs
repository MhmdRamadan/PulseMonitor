using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace PulseMonitor.API.Hubs;

[Authorize]
public class MonitoringHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, "Dashboard");
        await Clients.Others.SendAsync("UserJoined", Context.User?.Identity?.Name ?? "Anonymous");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Dashboard");
        await Clients.Others.SendAsync("UserLeft", Context.User?.Identity?.Name ?? "Anonymous");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinServerGroup(Guid serverId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Server_{serverId}");
    }

    public async Task LeaveServerGroup(Guid serverId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Server_{serverId}");
    }
}
