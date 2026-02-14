namespace PulseMonitor.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredAtUtc { get; }
}
