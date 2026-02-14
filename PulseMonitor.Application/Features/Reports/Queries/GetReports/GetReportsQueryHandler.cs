using MediatR;
using Microsoft.EntityFrameworkCore;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Application.Common.Models;

namespace PulseMonitor.Application.Features.Reports.Queries.GetReports;

public class GetReportsQueryHandler : IRequestHandler<GetReportsQuery, PagedResult<ReportListItemDto>>
{
    private readonly IApplicationDbContext _db;

    public GetReportsQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<ReportListItemDto>> Handle(GetReportsQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Reports
            .AsNoTracking()
            .Include(r => r.Server)
            .AsQueryable();

        if (request.ServerId.HasValue)
            query = query.Where(r => r.ServerId == request.ServerId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(r => r.CreatedAtUtc)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new ReportListItemDto(
                r.Id,
                r.ServerId,
                r.Server.Name,
                r.Status.ToString(),
                r.FromUtc,
                r.ToUtc,
                r.CreatedAtUtc,
                r.CompletedAtUtc))
            .ToListAsync(cancellationToken);

        return new PagedResult<ReportListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
