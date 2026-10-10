using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chima.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> b)
    {
        b.ToTable("notifications");
        b.HasKey(x => x.Id);
        b.Property(x => x.Type).HasConversion<int>();
        b.Property(x => x.Title).HasMaxLength(200).IsRequired();
        b.Property(x => x.Body).HasMaxLength(2000).IsRequired();
        b.Property(x => x.ActionUrl).HasMaxLength(500);
        b.HasIndex(x => new { x.UserId, x.IsRead });
        b.HasIndex(x => x.CreatedAt);

        b.HasOne(x => x.User).WithMany(u => u.Notifications)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
{
    public void Configure(EntityTypeBuilder<DeviceToken> b)
    {
        b.ToTable("device_tokens");
        b.HasKey(x => x.Id);
        b.Property(x => x.Token).HasMaxLength(500).IsRequired();
        b.HasIndex(x => x.Token).IsUnique();
        b.HasIndex(x => x.UserId);
        b.HasOne(x => x.User).WithMany(u => u.DeviceTokens)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> b)
    {
        b.ToTable("reports");
        b.HasKey(x => x.Id);
        b.Property(x => x.Reason).HasMaxLength(200).IsRequired();
        b.Property(x => x.Details).HasMaxLength(2000);
        b.Property(x => x.Resolution).HasMaxLength(2000);
        b.Property(x => x.Status).HasConversion<int>();
        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.ReporterUserId);
    }
}

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> b)
    {
        b.ToTable("feedback");
        b.HasKey(x => x.Id);
        b.Property(x => x.Category).HasMaxLength(64).IsRequired();
        b.Property(x => x.Body).HasMaxLength(5000).IsRequired();
        b.Property(x => x.ContactEmail).HasMaxLength(320);
        b.HasIndex(x => x.CreatedAt);
    }
}

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> b)
    {
        b.ToTable("system_settings");
        b.HasKey(x => x.Id);
        b.Property(x => x.Key).HasMaxLength(120).IsRequired();
        b.HasIndex(x => x.Key).IsUnique();
        b.Property(x => x.ValueType).HasMaxLength(32);
        b.Property(x => x.Description).HasMaxLength(500);
    }
}

public class FeatureFlagConfiguration : IEntityTypeConfiguration<FeatureFlag>
{
    public void Configure(EntityTypeBuilder<FeatureFlag> b)
    {
        b.ToTable("feature_flags");
        b.HasKey(x => x.Id);
        b.Property(x => x.Key).HasMaxLength(120).IsRequired();
        b.HasIndex(x => x.Key).IsUnique();
        b.Property(x => x.DisplayName).HasMaxLength(200);
        b.Property(x => x.Description).HasMaxLength(500);
        b.Property(x => x.EnabledForPlansCsv).HasMaxLength(255);
    }
}
