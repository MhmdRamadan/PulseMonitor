using PulseMonitor.Domain.Enums;

namespace PulseMonitor.Domain.Entities;

public class Alert : BaseEntity
{
    public Guid ServerId { get; set; }
    public string MetricType { get; set; } = string.Empty;
    public double MetricValue { get; set; }
    public double Threshold { get; set; }
    public string Status { get; set; } = "Triggered";
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? ResolvedAt { get; set; }

    public string Message { get; set; } = string.Empty;
    public AlertSeverity Severity { get; set; }
    public double? ThresholdValue { get; set; }
    public DateTime TriggeredAtUtc { get; set; }
    public DateTime? AcknowledgedAtUtc { get; set; }

    public Server Server { get; set; } = null!;
}
