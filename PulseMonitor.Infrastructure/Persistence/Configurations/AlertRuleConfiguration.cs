using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PulseMonitor.Domain.Entities;

namespace PulseMonitor.Infrastructure.Persistence.Configurations;

public class AlertRuleConfiguration : IEntityTypeConfiguration<AlertRule>
{
    public void Configure(EntityTypeBuilder<AlertRule> b)
    {
        b.ToTable("AlertRules");
        b.HasKey(x => x.Id);
        b.Property(x => x.Operator).HasMaxLength(10).IsRequired();
        b.Property(x => x.ThresholdValue).HasPrecision(10, 2);
    }
}
