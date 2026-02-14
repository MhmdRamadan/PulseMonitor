using MediatR;

namespace PulseMonitor.Application.Features.Metrics.Queries.GetMetricsByServer;

public record GetMetricsByServerQuery(
    Guid ServerId,
    DateTime? FromUtc,
    DateTime? ToUtc,
    int Limit = 100
) : IRequest<IReadOnlyList<MetricDto>>;

public record MetricDto(
    Guid Id,
    Guid ServerId,
    double CpuUsagePercent,
    double MemoryUsagePercent,
    double DiskUsagePercent,
    double ResponseTimeMs,
    string Status,
    DateTime TimestampUtc
);
