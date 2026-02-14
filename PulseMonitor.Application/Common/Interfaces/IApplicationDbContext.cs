using PulseMonitor.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace PulseMonitor.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Server> Servers { get; }
    DbSet<Metric> Metrics { get; }
    DbSet<User> Users { get; }
    DbSet<Role> Roles { get; }
    DbSet<UserRole> UserRoles { get; }
    DbSet<Disk> Disks { get; }
    DbSet<Alert> Alerts { get; }
    DbSet<AlertRule> AlertRules { get; }
    DbSet<Report> Reports { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
