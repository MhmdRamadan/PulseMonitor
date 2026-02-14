using PulseMonitor.Domain.Entities;

namespace PulseMonitor.Domain.Events;

public sealed class MetricCollectedEvent : IDomainEvent
{
    public Metric Metric { get; }
    public DateTime OccurredAtUtc { get; }

    public MetricCollectedEvent(Metric metric)
    {
        Metric = metric;
        OccurredAtUtc = DateTime.UtcNow;
    }
}
