namespace PulseMonitor.Application.Common.Interfaces;

public interface ISystemMetricsProvider
{
    bool IsAvailable { get; }

    (double CpuPercent, double MemoryPercent, double DiskUsedPercent, double? ResponseTimeMs) GetCurrentSnapshot();

    IReadOnlyList<DiskDriveSnapshot> GetDiskDrives();
}

public record DiskDriveSnapshot(string DriveLetter, long FreeSpaceMb, long TotalSpaceMb, double UsedPercentage);
