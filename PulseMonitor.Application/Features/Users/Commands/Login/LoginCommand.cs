using MediatR;
using PulseMonitor.Application.Common.Models;
using PulseMonitor.Application.Features.Users.Commands.Register;

namespace PulseMonitor.Application.Features.Users.Commands.Login;

public record LoginCommand(string UserName, string Password) : IRequest<Result<AuthResponseDto>>;
