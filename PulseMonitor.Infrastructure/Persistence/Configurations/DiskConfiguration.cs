using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseMonitor.Domain.Entities;

namespace PulseMonitor.Infrastructure.Persistence.Configurations;

public class DiskConfiguration : IEntityTypeConfiguration<Disk>
{
    public void Configure(EntityTypeBuilder<Disk> b)
    {
        b.ToTable("Disks");
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.ServerId, x.TimestampUtc });
        b.Property(x => x.DriveLetter).HasMaxLength(5).IsRequired();
        b.Property(x => x.UsedPercentage).HasPrecision(5, 2);
        b.HasOne(x => x.Server).WithMany(x => x.Disks).HasForeignKey(x => x.ServerId).OnDelete(DeleteBehavior.Cascade);
    }
}
