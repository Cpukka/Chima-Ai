using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chima.Infrastructure.Persistence.Configurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> b)
    {
        b.ToTable("subscription_plans");
        b.HasKey(x => x.Id);
        b.Property(x => x.Code).HasMaxLength(32).IsRequired();
        b.HasIndex(x => x.Code).IsUnique();
        b.Property(x => x.DisplayName).HasMaxLength(120).IsRequired();
        b.Property(x => x.Description).HasMaxLength(500);
        b.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        b.Property(x => x.MonthlyPrice).HasPrecision(18, 2);
        b.Property(x => x.YearlyPrice).HasPrecision(18, 2);
    }
}

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> b)
    {
        b.ToTable("user_subscriptions");
        b.HasKey(x => x.Id);
        b.Property(x => x.Status).HasConversion<int>();
        b.Property(x => x.Provider).HasConversion<int>();
        b.Property(x => x.ExternalSubscriptionId).HasMaxLength(200);
        b.Property(x => x.ExternalPurchaseToken).HasMaxLength(1000);
        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.ExternalSubscriptionId);

        b.HasOne(x => x.User).WithMany(u => u.Subscriptions)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Plan).WithMany(p => p.Subscriptions)
            .HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b)
    {
        b.ToTable("payments");
        b.HasKey(x => x.Id);
        b.Property(x => x.Provider).HasConversion<int>();
        b.Property(x => x.Status).HasConversion<int>();
        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        b.Property(x => x.ExternalPaymentId).HasMaxLength(200);
        b.Property(x => x.ExternalOrderId).HasMaxLength(200);
        b.Property(x => x.ReceiptUrl).HasMaxLength(500);
        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.ExternalPaymentId);

        b.HasOne(x => x.User).WithMany(u => u.Payments)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Subscription).WithMany()
            .HasForeignKey(x => x.SubscriptionId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class CreditTransactionConfiguration : IEntityTypeConfiguration<CreditTransaction>
{
    public void Configure(EntityTypeBuilder<CreditTransaction> b)
    {
        b.ToTable("credit_transactions");
        b.HasKey(x => x.Id);
        b.Property(x => x.Type).HasConversion<int>();
        b.Property(x => x.Amount).HasPrecision(18, 6);
        b.Property(x => x.BalanceAfter).HasPrecision(18, 6);
        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.CreatedAt);

        b.HasOne(x => x.User).WithMany(u => u.CreditTransactions)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
