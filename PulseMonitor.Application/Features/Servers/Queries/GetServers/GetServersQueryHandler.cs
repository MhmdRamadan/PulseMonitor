using MediatR;
using Microsoft.EntityFrameworkCore;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Application.Common.Models;
using PulseMonitor.Domain.Enums;

namespace PulseMonitor.Application.Features.Servers.Queries.GetServers;

public class GetServersQueryHandler : IRequestHandler<GetServersQuery, PagedResult<ServerListItemDto>>
{
    private readonly IApplicationDbContext _db;

    public GetServersQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<ServerListItemDto>> Handle(GetServersQuery request, CancellationToken cancellationToken)
    {
        var query = _db.Servers.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim().ToLower();
            query = query.Where(x =>
                x.Name.ToLower().Contains(s) ||
                (x.HostName != null && x.HostName.ToLower().Contains(s)) ||
                (x.IpAddress != null && x.IpAddress.ToLower().Contains(s)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        query = request.SortBy?.ToLowerInvariant() switch
        {
            "name" => request.SortDescending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name),
            "hostname" => request.SortDescending ? query.OrderByDescending(x => x.HostName) : query.OrderBy(x => x.HostName),
            "status" => request.SortDescending ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            _ => query.OrderBy(x => x.Name)
        };

        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new ServerListItemDto(
                s.Id,
                s.Name,
                s.HostName,
                s.IpAddress,
                s.Status.ToString(),
                s.Metrics.OrderByDescending(m => m.TimestampUtc).Select(m => m.TimestampUtc).FirstOrDefault()))
            .ToListAsync(cancellationToken);

        return new PagedResult<ServerListItemDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
    }
}
