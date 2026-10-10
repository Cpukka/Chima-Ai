using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chima.Infrastructure.Persistence.Configurations;

public class UsageRecordConfiguration : IEntityTypeConfiguration<UsageRecord>
{
    public void Configure(EntityTypeBuilder<UsageRecord> b)
    {
        b.ToTable("usage_records");
        b.HasKey(x => x.Id);
        b.Property(x => x.EstimatedCost).HasPrecision(18, 6);
        b.HasIndex(x => new { x.UserId, x.CreatedAt });
        b.HasIndex(x => x.ModelId);
        b.HasIndex(x => x.RequestId);

        b.HasOne(x => x.User).WithMany(u => u.UsageRecords)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Model).WithMany()
            .HasForeignKey(x => x.ModelId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class UsageLimitConfiguration : IEntityTypeConfiguration<UsageLimit>
{
    public void Configure(EntityTypeBuilder<UsageLimit> b)
    {
        b.ToTable("usage_limits");
        b.HasKey(x => x.Id);
        b.Property(x => x.MonthlyCreditAllowance).HasPrecision(18, 6);
        b.HasIndex(x => x.PlanId).IsUnique();

        b.HasOne(x => x.Plan).WithOne(p => p.UsageLimit)
            .HasForeignKey<UsageLimit>(x => x.PlanId).OnDelete(DeleteBehavior.Cascade);
    }
}
