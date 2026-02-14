using PulseMonitor.Application.Features.Metrics.Queries.GetMetricsByServer;

namespace PulseMonitor.Application.Common.Interfaces;

public interface IMetricPushService
{
    Task PushMetricAsync(MetricDto metric, CancellationToken cancellationToken = default);
    Task PushAlertAsync(Guid serverId, string serverName, string message, string severity, DateTime triggeredAt, CancellationToken cancellationToken = default);
}
