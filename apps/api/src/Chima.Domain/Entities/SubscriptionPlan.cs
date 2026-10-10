using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class SubscriptionPlan : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }
    public int SortOrder { get; set; }

    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    public string Currency { get; set; } = "USD";

    public string? GooglePlayMonthlyProductId { get; set; }
    public string? GooglePlayYearlyProductId { get; set; }
    public string? StripeMonthlyPriceId { get; set; }
    public string? StripeYearlyPriceId { get; set; }
    public string? PaystackMonthlyPlanCode { get; set; }
    public string? PaystackYearlyPlanCode { get; set; }

    public UsageLimit? UsageLimit { get; set; }
    public ICollection<UserSubscription> Subscriptions { get; set; } = new List<UserSubscription>();
}
