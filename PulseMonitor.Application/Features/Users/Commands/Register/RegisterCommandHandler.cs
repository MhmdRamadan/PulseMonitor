using MediatR;
using Microsoft.EntityFrameworkCore;
using PulseMonitor.Application.Common.Interfaces;
using PulseMonitor.Application.Common.Models;
using PulseMonitor.Domain.Entities;

namespace PulseMonitor.Application.Features.Users.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenService _jwt;

    public RegisterCommandHandler(IApplicationDbContext db, IPasswordHasher hasher, IJwtTokenService jwt)
    {
        _db = db;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _db.Users.AnyAsync(u => u.UserName == request.UserName, cancellationToken))
            return Result<AuthResponseDto>.Failure("UserName already exists.");
        if (await _db.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
            return Result<AuthResponseDto>.Failure("Email already registered.");

        var userRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "User", cancellationToken);
        if (userRole == null)
            return Result<AuthResponseDto>.Failure("Default role not configured.");

        var user = new User
        {
            UserName = request.UserName,
            Email = request.Email,
            PasswordHash = _hasher.Hash(request.Password),
            RoleId = userRole.Id,
            CreatedAtUtc = DateTime.UtcNow
        };
        _db.Users.Add(user);
        await _db.SaveChangesAsync(cancellationToken);

        _db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = userRole.Id });
        await _db.SaveChangesAsync(cancellationToken);

        var roles = new[] { "User" };
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
