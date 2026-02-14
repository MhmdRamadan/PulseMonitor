using Microsoft.EntityFrameworkCore;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Application.Features.Metrics.Queries.GetMetricsByServer;
using PulseMonitor.Domain.Entities;
using PulseMonitor.Domain.Enums;

namespace PulseMonitor.Infrastructure.BackgroundJobs;

public class MetricsCollectionJob
{
    private readonly IApplicationDbContext _db;
    private readonly IMetricPushService _push;
    private readonly ISystemMetricsProvider? _systemMetrics;
    private static readonly Random Rng = new();

    public MetricsCollectionJob(IApplicationDbContext db, IMetricPushService push, ISystemMetricsProvider? systemMetrics = null)
    {
        _db = db;
        _push = push;
        _systemMetrics = systemMetrics;
    }

    public async Task ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var serverIds = await _db.Servers.Select(s => s.Id).ToListAsync(cancellationToken);
        foreach (var serverId in serverIds)
        {
            var metric = await CollectAndSaveMetricAsync(serverId, cancellationToken);
            if (metric != null)
            {
                await _push.PushMetricAsync(new MetricDto(
                    metric.Id,
                    metric.ServerId,
                    metric.CpuUsagePercent,
                    metric.MemoryUsagePercent,
                    metric.DiskUsagePercent,
                    metric.ResponseTimeMs,
                    metric.Status.ToString(),
                    metric.TimestampUtc
                ), cancellationToken);
                await EvaluateAlertRulesAsync(serverId, metric, cancellationToken);
            }
        }
    }

    private async Task<Metric?> CollectAndSaveMetricAsync(Guid serverId, CancellationToken cancellationToken)
    {
        var server = await _db.Servers.FindAsync(new object[] { serverId }, cancellationToken);
        double cpu, memory, disk, responseMs;
        ServerStatus status;

        bool useRealMetrics = _systemMetrics?.IsAvailable == true && server != null &&
            (string.Equals(server.HostName, "localhost", StringComparison.OrdinalIgnoreCase) ||
             string.Equals(server.HostName, ".", StringComparison.Ordinal) ||
             string.Equals(server.Name, "Local", StringComparison.OrdinalIgnoreCase));

        if (useRealMetrics)
        {
            var (cpuPct, memPct, diskPct, responseTime) = _systemMetrics!.GetCurrentSnapshot();
            cpu = cpuPct;
            memory = memPct;
            disk = diskPct;
            responseMs = responseTime ?? 0;
            status = ServerStatus.Up;
        }
        else
        {
            status = Rng.NextDouble() > 0.1 ? ServerStatus.Up : ServerStatus.Down;
            cpu = Math.Round(Rng.NextDouble() * 100, 2);
            memory = Math.Round(Rng.NextDouble() * 100, 2);
            disk = Math.Round(30 + Rng.NextDouble() * 60, 2);
            responseMs = Math.Round(Rng.NextDouble() * 500, 2);
        }

        var metric = new Metric
        {
            ServerId = serverId,
            CpuUsagePercent = cpu,
            MemoryUsagePercent = memory,
            DiskUsagePercent = disk,
            ResponseTimeMs = responseMs,
            Status = status,
            TimestampUtc = DateTime.UtcNow
        };
        _db.Metrics.Add(metric);

        if (server != null)
        {
            server.Status = status;
            server.UpdatedAtUtc = DateTime.UtcNow;
        }

        if (useRealMetrics && server != null && _systemMetrics != null)
        {
            foreach (var drive in _systemMetrics.GetDiskDrives())
            {
                _db.Disks.Add(new Disk
                {
                    ServerId = serverId,
                    DriveLetter = drive.DriveLetter,
                    FreeSpaceMb = drive.FreeSpaceMb,
                    TotalSpaceMb = drive.TotalSpaceMb,
                    UsedPercentage = drive.UsedPercentage,
                    TimestampUtc = DateTime.UtcNow
                });
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
        return metric;
    }

    private async Task EvaluateAlertRulesAsync(Guid serverId, Metric metric, CancellationToken cancellationToken)
    {
        var rules = await _db.AlertRules
            .Where(r => r.ServerId == serverId && r.IsActive)
            .ToListAsync(cancellationToken);
        var server = await _db.Servers.FindAsync(new object[] { serverId }, cancellationToken);
        var serverName = server?.Name ?? serverId.ToString();

        foreach (var rule in rules)
        {
            var value = rule.MetricType switch
            {
                MetricType.CpuUsage => metric.CpuUsagePercent,
                MetricType.MemoryUsage => metric.MemoryUsagePercent,
                MetricType.DiskUsage => metric.DiskUsagePercent,
                MetricType.ResponseTime => metric.ResponseTimeMs,
                _ => 0
            };
            var breached = rule.Operator switch
            {
                ">" => value > rule.ThresholdValue,
                ">=" => value >= rule.ThresholdValue,
                "<" => value < rule.ThresholdValue,
                "<=" => value <= rule.ThresholdValue,
                _ => false
            };
            if (!breached) continue;

            var alert = new Alert
            {
                ServerId = serverId,
                MetricType = rule.MetricType.ToString(),
                MetricValue = value,
                Threshold = rule.ThresholdValue,
                Status = "Triggered",
                CreatedAtUtc = DateTime.UtcNow,
                Message = $"{rule.MetricType} {value} {rule.Operator} {rule.ThresholdValue}",
                Severity = rule.Severity,
                ThresholdValue = rule.ThresholdValue,
                TriggeredAtUtc = DateTime.UtcNow
            };
            _db.Alerts.Add(alert);
            await _db.SaveChangesAsync(cancellationToken);
            await _push.PushAlertAsync(serverId, serverName, alert.Message, alert.Severity.ToString(), alert.TriggeredAtUtc, cancellationToken);
        }
    }
}
