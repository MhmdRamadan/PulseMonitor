using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PulseMonitor.Application.Common.Interfaces;

namespace PulseMonitor.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;
    private static readonly Dictionary<string, (string UserId, string UserName)> RefreshStore = new();

    public JwtTokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateAccessToken(string userId, string userName, IReadOnlyList<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"] ?? "PulseMonitorSecretKeyAtLeast32CharactersLong!!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, userName)
        };
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "PulseMonitor",
            audience: _config["Jwt:Audience"] ?? "PulseMonitor",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string? GenerateRefreshToken()
    {
        var bytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    public void StoreRefreshToken(string refreshToken, string userId, string userName)
    {
        RefreshStore[refreshToken] = (userId, userName);
    }

    public (string? userId, string? userName) ValidateRefreshToken(string refreshToken)
    {
        if (RefreshStore.TryGetValue(refreshToken, out var data))
        {
            RefreshStore.Remove(refreshToken);
            return (data.UserId, data.UserName);
        }
        return (null, null);
    }

}
