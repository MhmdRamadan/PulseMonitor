using MediatR;
using PulseMonitor.Application.Common.Models;

namespace PulseMonitor.Application.Features.Users.Queries.GetUsers;

public record GetUsersQuery(int PageNumber = 1, int PageSize = 20) : IRequest<PagedResult<UserListItemDto>>;

public record UserListItemDto(Guid Id, string UserName, string Email, DateTime CreatedAtUtc, IReadOnlyList<string> Roles);
