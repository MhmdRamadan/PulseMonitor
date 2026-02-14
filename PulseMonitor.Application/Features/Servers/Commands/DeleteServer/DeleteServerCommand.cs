using MediatR;

namespace PulseMonitor.Application.Features.Servers.Commands.DeleteServer;

public record DeleteServerCommand(Guid Id) : IRequest<Unit>;
