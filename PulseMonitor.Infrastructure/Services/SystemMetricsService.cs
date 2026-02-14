using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using PulseMonitor.Application.Common.Interfaces;

namespace PulseMonitor.Infrastructure.Services;

[SupportedOSPlatform("windows")]
public class SystemMetricsService : ISystemMetricsProvider
{
    private readonly PerformanceCounter? _cpuCounter;
    private readonly PerformanceCounter? _ramCounter;
    private readonly PerformanceCounter? _diskCounter;

    public SystemMetricsService()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            _cpuCounter = null;
            _ramCounter = null;
            _diskCounter = null;
            return;
        }

        try
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            _diskCounter = new PerformanceCounter("LogicalDisk", "% Free Space", "_Total");
            _cpuCounter.NextValue();
            _ramCounter.NextValue();
            _diskCounter.NextValue();
        }
        catch
        {
            _cpuCounter = null;
            _ramCounter = null;
            _diskCounter = null;
        }
    }

    public bool IsAvailable => _cpuCounter != null && _ramCounter != null && _diskCounter != null;

    public (double CpuPercent, double MemoryPercent, double DiskUsedPercent, double? ResponseTimeMs) GetCurrentSnapshot()
    {
        if (!IsAvailable)
            return (0, 0, 0, null);

        try
        {
            var cpu = (float)_cpuCounter!.NextValue();
            Thread.Sleep(100);
            cpu = (float)_cpuCounter.NextValue();

            var memory = (float)_ramCounter!.NextValue();
            var diskFree = (float)_diskCounter!.NextValue();
            var diskUsed = 100.0 - diskFree;

            return (Math.Round(cpu, 2), Math.Round(memory, 2), Math.Round(diskUsed, 2), null);
        }
        catch
        {
            return (0, 0, 0, null);
        }
    }

    public IReadOnlyList<DiskDriveSnapshot> GetDiskDrives()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            return Array.Empty<DiskDriveSnapshot>();

        var list = new List<DiskDriveSnapshot>();
        try
        {
            foreach (var drive in DriveInfo.GetDrives().Where(d => d.IsReady && d.DriveType == DriveType.Fixed))
            {
                var total = drive.TotalSize / (1024 * 1024);
                var free = drive.AvailableFreeSpace / (1024 * 1024);
                var usedPct = total > 0 ? 100.0 * (total - free) / total : 0;
                list.Add(new DiskDriveSnapshot(
                    drive.Name.TrimEnd('\\'),
                    free,
                    total,
                    Math.Round(usedPct, 2)));
            }
        }
        catch
        {
        }

        return list;
    }
}
