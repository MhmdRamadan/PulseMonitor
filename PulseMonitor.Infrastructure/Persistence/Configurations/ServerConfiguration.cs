using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseMonitor.Domain.Entities;

namespace PulseMonitor.Infrastructure.Persistence.Configurations;

public class ServerConfiguration : IEntityTypeConfiguration<Server>
{
    public void Configure(EntityTypeBuilder<Server> b)
    {
        b.ToTable("Servers");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        b.Property(x => x.HostName).HasMaxLength(500).IsRequired();
        b.Property(x => x.IpAddress).HasMaxLength(50);
        b.Property(x => x.Description).HasMaxLength(250);
        b.HasMany(x => x.Disks).WithOne(x => x.Server).HasForeignKey(x => x.ServerId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.Metrics).WithOne(x => x.Server).HasForeignKey(x => x.ServerId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.Alerts).WithOne(x => x.Server).HasForeignKey(x => x.ServerId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.AlertRules).WithOne(x => x.Server).HasForeignKey(x => x.ServerId).OnDelete(DeleteBehavior.Cascade);
        b.HasMany(x => x.Reports).WithOne(x => x.Server).HasForeignKey(x => x.ServerId).OnDelete(DeleteBehavior.Restrict);
    }
}
