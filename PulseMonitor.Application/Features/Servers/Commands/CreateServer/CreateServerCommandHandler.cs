using MediatR;
using PulseMonitor.Domain.Entities;
using PulseMonitor.Domain.Enums;
using PulseMonitor.Application.Common.Interfaces;

namespace PulseMonitor.Application.Features.Servers.Commands.CreateServer;

public class CreateServerCommandHandler : IRequestHandler<CreateServerCommand, Guid>
{
    private readonly IApplicationDbContext _db;

    public CreateServerCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> Handle(CreateServerCommand request, CancellationToken cancellationToken)
    {
        var server = new Server
        {
            Name = request.Name,
            HostName = request.HostName,
            IpAddress = request.IpAddress,
            Description = request.Description,
            Status = ServerStatus.Unknown,
            CreatedAtUtc = DateTime.UtcNow
        };
        _db.Servers.Add(server);
        await _db.SaveChangesAsync(cancellationToken);
        return server.Id;
    }
}
