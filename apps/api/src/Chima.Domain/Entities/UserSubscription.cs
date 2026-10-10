using Chima.Domain.Common;
using Chima.Domain.Enums;

namespace Chima.Domain.Entities;

public class UserSubscription : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid PlanId { get; set; }
    public SubscriptionPlan Plan { get; set; } = null!;

    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? EndedAt { get; set; }

    public PaymentProvider Provider { get; set; }
    public string? ExternalSubscriptionId { get; set; }
    public string? ExternalPurchaseToken { get; set; }
    public bool AutoRenew { get; set; } = true;
    public DateTime? GracePeriodEndsAt { get; set; }
}
