using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseMonitor.Domain.Entities;

namespace PulseMonitor.Infrastructure.Persistence.Configurations;

public class AlertConfiguration : IEntityTypeConfiguration<Alert>
{
    public void Configure(EntityTypeBuilder<Alert> b)
    {
        b.ToTable("Alerts");
        b.HasKey(x => x.Id);
        b.HasIndex(x => new { x.ServerId, x.CreatedAtUtc });
        b.Property(x => x.MetricType).HasMaxLength(50).IsRequired();
        b.Property(x => x.Status).HasMaxLength(20).IsRequired();
        b.Property(x => x.Message).HasMaxLength(1000);
        b.Property(x => x.MetricValue).HasPrecision(10, 2);
        b.Property(x => x.Threshold).HasPrecision(10, 2);
        b.Property(x => x.ThresholdValue).HasPrecision(10, 2);
    }
}
