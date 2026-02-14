using MediatR;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Domain.Exceptions;

namespace PulseMonitor.Application.Features.Servers.Commands.UpdateServer;

public class UpdateServerCommandHandler : IRequestHandler<UpdateServerCommand, Unit>
{
    private readonly IApplicationDbContext _db;

    public UpdateServerCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(UpdateServerCommand request, CancellationToken cancellationToken)
    {
        var server = await _db.Servers.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Server), request.Id);
        server.Name = request.Name;
        server.HostName = request.HostName;
        server.IpAddress = request.IpAddress;
        server.Description = request.Description;
        server.UpdatedAtUtc = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
