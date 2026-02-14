using PulseMonitor.Domain.Enums;

namespace PulseMonitor.Domain.Entities;

public class Report : BaseEntity
{
    public Guid ServerId { get; set; }
    public Guid RequestedByUserId { get; set; }
    public ReportStatus Status { get; set; } = ReportStatus.Pending;
    public DateTime FromUtc { get; set; }
    public DateTime ToUtc { get; set; }
    public string? FilePath { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public string? ErrorMessage { get; set; }

    public Server Server { get; set; } = null!;
    public User RequestedByUser { get; set; } = null!;
}
