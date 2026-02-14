using MediatR;

namespace PulseMonitor.Application.Features.Servers.Queries.GetServerById;

public record GetServerByIdQuery(Guid Id) : IRequest<ServerDetailDto?>;

public record ServerDetailDto(
    Guid Id,
    string Name,
    string HostName,
    string? IpAddress,
    string? Description,
    string Status,
    DateTime CreatedAtUtc,
    DateTime? UpdatedAtUtc
);
