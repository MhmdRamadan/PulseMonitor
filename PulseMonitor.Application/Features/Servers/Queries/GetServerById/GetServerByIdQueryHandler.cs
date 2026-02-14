using MediatR;
using Microsoft.EntityFrameworkCore;
using PulseMonitor.Application.Common.Interfaces;

namespace PulseMonitor.Application.Features.Servers.Queries.GetServerById;

public class GetServerByIdQueryHandler : IRequestHandler<GetServerByIdQuery, ServerDetailDto?>
{
    private readonly IApplicationDbContext _db;

    public GetServerByIdQueryHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<ServerDetailDto?> Handle(GetServerByIdQuery request, CancellationToken cancellationToken)
    {
        var server = await _db.Servers
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
        if (server == null) return null;
        return new ServerDetailDto(
            server.Id,
            server.Name,
            server.HostName,
            server.IpAddress,
            server.Description,
            server.Status.ToString(),
            server.CreatedAtUtc,
            server.UpdatedAtUtc
        );
    }
}
