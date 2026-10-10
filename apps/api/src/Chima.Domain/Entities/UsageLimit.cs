using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class UsageLimit : BaseEntity
{
    public Guid PlanId { get; set; }
    public SubscriptionPlan Plan { get; set; } = null!;

    public int MonthlyMessages { get; set; }
    public int MonthlyFileUploads { get; set; }
    public int MonthlyTokens { get; set; }
    public int MaxFileSizeMb { get; set; } = 25;
    public int MaxConversations { get; set; } = -1;
    public decimal MonthlyCreditAllowance { get; set; }
}
