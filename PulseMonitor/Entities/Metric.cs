using PulseMonitor.Domain.Enums;

namespace PulseMonitor.Domain.Entities;

public class Metric : BaseEntity
{
    public Guid ServerId { get; set; }
    public double CpuUsagePercent { get; set; }
    public double MemoryUsagePercent { get; set; }
    public double DiskUsagePercent { get; set; }
    public double ResponseTimeMs { get; set; }
    public ServerStatus Status { get; set; }
    public DateTime TimestampUtc { get; set; }

    public Server Server { get; set; } = null!;
}
