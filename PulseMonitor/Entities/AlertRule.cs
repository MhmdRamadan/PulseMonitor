using PulseMonitor.Domain.Enums;

namespace PulseMonitor.Domain.Entities;

public class AlertRule : BaseEntity
{
    public Guid ServerId { get; set; }
    public MetricType MetricType { get; set; }
    public string Operator { get; set; } = ">";
    public double ThresholdValue { get; set; }
    public AlertSeverity Severity { get; set; }
    public bool IsActive { get; set; } = true;

    public Server Server { get; set; } = null!;
}
