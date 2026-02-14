using MediatR;

namespace PulseMonitor.Application.Features.Servers.Commands.UpdateServer;

public record UpdateServerCommand(Guid Id, string Name, string HostName, string? IpAddress, string? Description) : IRequest<Unit>;
