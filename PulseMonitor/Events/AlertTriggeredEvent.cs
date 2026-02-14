using PulseMonitor.Domain.Entities;

namespace PulseMonitor.Domain.Events;

public sealed class AlertTriggeredEvent : IDomainEvent
{
    public Alert Alert { get; }
    public DateTime OccurredAtUtc { get; }

    public AlertTriggeredEvent(Alert alert)
    {
        Alert = alert;
        OccurredAtUtc = DateTime.UtcNow;
    }
}
