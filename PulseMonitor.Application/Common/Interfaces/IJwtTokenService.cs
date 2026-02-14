namespace PulseMonitor.Application.Common.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(string userId, string userName, IReadOnlyList<string> roles);
    string? GenerateRefreshToken();
    void StoreRefreshToken(string refreshToken, string userId, string userName);
    (string? userId, string? userName) ValidateRefreshToken(string refreshToken);
}
