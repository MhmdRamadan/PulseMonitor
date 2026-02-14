namespace PulseMonitor.Domain.Entities;

public class Disk : BaseEntity
{
    public Guid ServerId { get; set; }
    public string DriveLetter { get; set; } = string.Empty;
    public long FreeSpaceMb { get; set; }
    public long TotalSpaceMb { get; set; }
    public double UsedPercentage { get; set; }
    public DateTime TimestampUtc { get; set; }

    public Server Server { get; set; } = null!;
}
