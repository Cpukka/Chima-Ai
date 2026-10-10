using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chima.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users");
        b.HasKey(x => x.Id);
        b.Property(x => x.Email).HasMaxLength(320).IsRequired();
        b.HasIndex(x => x.Email).IsUnique();
        b.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
        b.Property(x => x.Status).HasConversion<int>();
        b.Property(x => x.GoogleSubjectId).HasMaxLength(128);
        b.HasIndex(x => x.GoogleSubjectId);

        b.HasOne(x => x.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<UserProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> b)
    {
        b.ToTable("user_profiles");
        b.HasKey(x => x.Id);
        b.Property(x => x.DisplayName).HasMaxLength(120);
        b.Property(x => x.AvatarUrl).HasMaxLength(500);
        b.Property(x => x.TimeZone).HasMaxLength(64);
        b.Property(x => x.Language).HasMaxLength(16);
        b.Property(x => x.Theme).HasMaxLength(16);
        b.HasIndex(x => x.UserId).IsUnique();
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("refresh_tokens");
        b.HasKey(x => x.Id);
        b.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
        b.HasIndex(x => x.TokenHash).IsUnique();
        b.Property(x => x.DeviceInfo).HasMaxLength(500);
        b.Property(x => x.IpAddress).HasMaxLength(64);
        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.ExpiresAt);

        b.HasOne(x => x.User).WithMany(u => u.RefreshTokens)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> b)
    {
        b.ToTable("admin_users");
        b.HasKey(x => x.Id);
        b.Property(x => x.Role).HasConversion<int>();
        b.HasIndex(x => x.UserId).IsUnique();
        b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> b)
    {
        b.ToTable("audit_logs");
        b.HasKey(x => x.Id);
        b.Property(x => x.Action).HasMaxLength(120).IsRequired();
        b.Property(x => x.EntityType).HasMaxLength(120);
        b.Property(x => x.EntityId).HasMaxLength(120);
        b.Property(x => x.ActorEmail).HasMaxLength(320);
        b.Property(x => x.IpAddress).HasMaxLength(64);
        b.Property(x => x.UserAgent).HasMaxLength(500);
        b.HasIndex(x => x.CreatedAt);
        b.HasIndex(x => x.ActorUserId);
    }
}
