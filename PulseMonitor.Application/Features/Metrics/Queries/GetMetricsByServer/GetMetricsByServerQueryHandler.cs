using MediatR;
using Microsoft.EntityFrameworkCore;
using PulseMonitor.Application.Common.Interfaces;

namespace PulseMonitor.Application.Features.Metrics.Queries.GetMetricsByServer;

public class GetMetricsByServerQueryHandler : IRequestHandler<GetMetricsByServerQuery, IReadOnlyList<MetricDto>>
    {
    private readonly IApplicationDbContext _db;

    public GetMetricsByServerQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<MetricDto>> Handle(GetMetricsByServerQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Metrics
            .AsNoTracking()
            .Where(m => m.ServerId == request.ServerId);

        if (request.FromUtc.HasValue)
            query = query.Where(m => m.TimestampUtc >= request.FromUtc.Value);
        if (request.ToUtc.HasValue)
            query = query.Where(m => m.TimestampUtc <= request.ToUtc.Value);

        var list = await query
            .OrderByDescending(m => m.TimestampUtc)
            .Take(request.Limit)
            .Select(m => new MetricDto(
                m.Id,
                m.ServerId,
                m.CpuUsagePercent,
                m.MemoryUsagePercent,
                m.DiskUsagePercent,
                m.ResponseTimeMs,
                m.Status.ToString(),
                m.TimestampUtc))
            .ToListAsync(cancellationToken);

        return list;
    }
}
