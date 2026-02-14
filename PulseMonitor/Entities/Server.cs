using PulseMonitor.Domain.Enums;
using PulseMonitor.Domain.Interfaces;

namespace PulseMonitor.Domain.Entities;

public class Server : BaseEntity, IAuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string HostName { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public ServerStatus Status { get; set; } = ServerStatus.Unknown;

    public ICollection<Metric> Metrics { get; set; } = new List<Metric>();
    public ICollection<Disk> Disks { get; set; } = new List<Disk>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
    public ICollection<AlertRule> AlertRules { get; set; } = new List<AlertRule>();
    public ICollection<Report> Reports { get; set; } = new List<Report>();
}
