using MediatR;
using PulseMonitor.Application.Common.Models;

namespace PulseMonitor.Application.Features.Users.Commands.Register;

public record RegisterCommand(string UserName, string Email, string Password) : IRequest<Result<AuthResponseDto>>;

public record AuthResponseDto(string AccessToken, string RefreshToken, DateTime ExpiresAtUtc, string UserName, IReadOnlyList<string> Roles);
