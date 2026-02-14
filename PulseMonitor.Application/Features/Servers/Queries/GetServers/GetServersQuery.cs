using MediatR;
using PulseMonitor.Application.Common.Models;

namespace PulseMonitor.Application.Features.Servers.Queries.GetServers;

public record GetServersQuery(
    string? Search,
    int PageNumber = 1,
    int PageSize = 20,
    string? SortBy = "Name",
    bool SortDescending = false
) : IRequest<PagedResult<ServerListItemDto>>;

public record ServerListItemDto(
    Guid Id,
    string Name,
    string HostName,
    string? IpAddress,
    string Status,
    DateTime? LastMetricAt
);
