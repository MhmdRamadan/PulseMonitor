using MediatR;
using Microsoft.EntityFrameworkCore;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Application.Common.Models;
using PulseMonitor.Application.Features.Users.Commands.Register;

namespace PulseMonitor.Application.Features.Users.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;

    public LoginCommandHandler(IApplicationDbContext db, IPasswordHasher hasher, IJwtTokenService jwt)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users
            .Include(u => u.Role)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.UserName == request.UserName, cancellationToken);

        if (user == null || !_hasher.Verify(request.Password, user.PasswordHash))
            return Result<AuthResponseDto>.Failure("Invalid username or password.");

        var roles = user.Role != null
            ? new List<string> { user.Role.Name }
            : user.UserRoles.Select(ur => ur.Role?.Name).Where(n => n != null).Cast<string>().ToList();
        if (roles.Count == 0)
            roles.Add("User");
        var accessToken = _jwt.GenerateAccessToken(user.Id.ToString(), user.UserName, roles);
        var refreshToken = _jwt.GenerateRefreshToken() ?? string.Empty;
        _jwt.StoreRefreshToken(refreshToken, user.Id.ToString(), user.UserName);
        var expiresAt = DateTime.UtcNow.AddMinutes(15);

        return Result<AuthResponseDto>.Success(new AuthResponseDto(
            accessToken,
            refreshToken,
            expiresAt,
            user.UserName,
            roles
        ));
    }
}
