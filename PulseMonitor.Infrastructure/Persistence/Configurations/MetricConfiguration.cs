using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseMonitor.Domain.Entities;

namespace PulseMonitor.Infrastructure.Persistence.Configurations;

public class MetricConfiguration : IEntityTypeConfiguration<Metric>
{
    public void Configure(EntityTypeBuilder<Metric> b)
    {
        b.ToTable("Metrics");
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.ServerId, x.TimestampUtc });
        b.Property(x => x.CpuUsagePercent).HasPrecision(5, 2);
        b.Property(x => x.MemoryUsagePercent).HasPrecision(5, 2);
        b.Property(x => x.DiskUsagePercent).HasPrecision(5, 2);
        b.Property(x => x.ResponseTimeMs).HasPrecision(10, 2);
    }
}
