using MediatR;
using Microsoft.EntityFrameworkCore;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Application.Features.Metrics.Queries.GetMetricsByServer;

namespace PulseMonitor.Application.Features.Metrics.Queries.GetLatestMetric;

public class GetLatestMetricQueryHandler : IRequestHandler<GetLatestMetricQuery, MetricDto?>
{
    private readonly IApplicationDbContext _db;

    public GetLatestMetricQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<MetricDto?> Handle(GetLatestMetricQuery request, CancellationToken cancellationToken)
    {
        var m = await _db.Metrics
            .AsNoTracking()
            .Where(x => x.ServerId == request.ServerId)
            .OrderByDescending(x => x.TimestampUtc)
            .FirstOrDefaultAsync(cancellationToken);
        if (m == null) return null;
        return new MetricDto(
            m.Id,
            m.ServerId,
            m.CpuUsagePercent,
            m.MemoryUsagePercent,
            m.DiskUsagePercent,
            m.ResponseTimeMs,
            m.Status.ToString(),
            m.TimestampUtc
        );
    }
}
