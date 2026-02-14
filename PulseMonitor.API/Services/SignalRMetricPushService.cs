using Microsoft.AspNetCore.SignalR;
using PulseMonitor.API.Hubs;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Application.Features.Metrics.Queries.GetMetricsByServer;

namespace PulseMonitor.API.Services;

public class SignalRMetricPushService : IMetricPushService
{
    private readonly IHubContext<MonitoringHub> _hub;

    public SignalRMetricPushService(IHubContext<MonitoringHub> hub)
    {
        _hub = hub;
    }

    public async Task PushMetricAsync(MetricDto metric, CancellationToken cancellationToken = default)
    {
        await _hub.Clients.Group("Dashboard").SendAsync("MetricUpdate", metric, cancellationToken);
        await _hub.Clients.Group($"Server_{metric.ServerId}").SendAsync("MetricUpdate", metric, cancellationToken);
    }

    public async Task PushAlertAsync(Guid serverId, string serverName, string message, string severity, DateTime triggeredAt, CancellationToken cancellationToken = default)
    {
        var payload = new { serverId, serverName, message, severity, triggeredAt };
        await _hub.Clients.Group("Dashboard").SendAsync("Alert", payload, cancellationToken);
        await _hub.Clients.Group($"Server_{serverId}").SendAsync("Alert", payload, cancellationToken);
    }
}
