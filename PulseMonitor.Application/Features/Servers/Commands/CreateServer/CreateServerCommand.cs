using MediatR;
using PulseMonitor.Application.Features.Servers.Commands.CreateServer;

namespace PulseMonitor.Application.Features.Servers.Commands.CreateServer;

public record CreateServerCommand(string Name, string HostName, string? IpAddress, string? Description) : IRequest<Guid>;
