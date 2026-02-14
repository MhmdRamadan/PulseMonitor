using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PulseMonitor.Domain.Entities;
using PulseMonitor.Domain.Enums;

namespace PulseMonitor.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, ILogger logger)
    {
        if (await db.Roles.AnyAsync()) return;

        logger.LogInformation("Seeding database...");

        var adminRole = new Role { Id = Guid.NewGuid(), Name = "Admin", Description = "Administrator" };
        var userRole = new Role { Id = Guid.NewGuid(), Name = "User", Description = "Standard user" };
        db.Roles.AddRange(adminRole, userRole);

        var adminUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = "admin",
            Email = "admin@pulsemonitor.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!", BCrypt.Net.BCrypt.GenerateSalt(12)),
            RoleId = adminRole.Id,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Users.Add(adminUser);
        db.UserRoles.Add(new UserRole { UserId = adminUser.Id, RoleId = adminRole.Id });

        var demoUser = new User
        {
            Id = Guid.NewGuid(),
            UserName = "demo",
            Email = "demo@pulsemonitor.local",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo123!", BCrypt.Net.BCrypt.GenerateSalt(12)),
            RoleId = userRole.Id,
            CreatedAtUtc = DateTime.UtcNow
        };
        db.Users.Add(demoUser);
        db.UserRoles.Add(new UserRole { UserId = demoUser.Id, RoleId = userRole.Id });

        var now = DateTime.UtcNow;
        var servers = new[]
        {
            new Server { Id = Guid.NewGuid(), Name = "Web Server 01", HostName = "web01.internal", IpAddress = "192.168.1.10", Status = ServerStatus.Up, CreatedAtUtc = now },
            new Server { Id = Guid.NewGuid(), Name = "API Server 01", HostName = "api01.internal", IpAddress = "192.168.1.11", Status = ServerStatus.Up, CreatedAtUtc = now },
            new Server { Id = Guid.NewGuid(), Name = "DB Server 01", HostName = "db01.internal", IpAddress = "192.168.1.12", Status = ServerStatus.Up, CreatedAtUtc = now }
        };
        db.Servers.AddRange(servers);

        await db.SaveChangesAsync();
        logger.LogInformation("Database seeded.");
    }
}
