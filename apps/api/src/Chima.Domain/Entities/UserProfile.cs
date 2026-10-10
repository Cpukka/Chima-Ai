using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class UserProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public string Language { get; set; } = "en";
    public string Theme { get; set; } = "system";

    public bool NotifySubscriptionRenewal { get; set; } = true;
    public bool NotifyUsageWarning { get; set; } = true;
    public bool NotifySecurityAlert { get; set; } = true;
    public bool NotifyProductAnnouncements { get; set; } = true;
    public bool AnalyticsOptIn { get; set; } = true;
}
