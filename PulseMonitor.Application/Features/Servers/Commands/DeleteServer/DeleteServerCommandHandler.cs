using MediatR;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Domain.Exceptions;

namespace PulseMonitor.Application.Features.Servers.Commands.DeleteServer;

public class DeleteServerCommandHandler : IRequestHandler<DeleteServerCommand, Unit>
{
    private readonly IApplicationDbContext _db;

    public DeleteServerCommandHandler(IApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<Unit> Handle(DeleteServerCommand request, CancellationToken cancellationToken)
    {
        var server = await _db.Servers.FindAsync(new object[] { request.Id }, cancellationToken)
            ?? throw new NotFoundException(nameof(Domain.Entities.Server), request.Id);
        _db.Servers.Remove(server);
        await _db.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
