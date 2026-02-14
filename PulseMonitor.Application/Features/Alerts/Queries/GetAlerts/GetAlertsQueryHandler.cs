using MediatR;
using Microsoft.EntityFrameworkCore;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Application.Common.Models;

namespace PulseMonitor.Application.Features.Alerts.Queries.GetAlerts;

public class GetAlertsQueryHandler : IRequestHandler<GetAlertsQuery, PagedResult<AlertDto>>
{
    private readonly IApplicationDbContext _db;

    public GetAlertsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<AlertDto>> Handle(GetAlertsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Alerts
            .AsNoTracking()
            .Include(a => a.Server)
            .AsQueryable();

        if (request.ServerId.HasValue)
            query = query.Where(a => a.ServerId == request.ServerId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(a => a.TriggeredAtUtc)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(a => new AlertDto(
                a.Id,
                a.ServerId,
                a.Server.Name,
                a.Message,
                a.Severity.ToString(),
                a.ThresholdValue,
                a.TriggeredAtUtc,
                a.AcknowledgedAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<AlertDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
