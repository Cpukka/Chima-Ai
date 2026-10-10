namespace Chima.Application.DTOs.Subscription;

public class PlanDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    public string Currency { get; set; } = "USD";
    public int SortOrder { get; set; }
    public bool IsDefault { get; set; }

    public PlanLimitsDto Limits { get; set; } = new();
}

public class PlanLimitsDto
{
    public int MonthlyMessages { get; set; }
    public int MonthlyFileUploads { get; set; }
    public int MonthlyTokens { get; set; }
    public int MaxFileSizeMb { get; set; }
    public int MaxConversations { get; set; }
    public decimal MonthlyCreditAllowance { get; set; }
}

public class SubscriptionStatusDto
{
    public string PlanCode { get; set; } = "free";
    public string PlanName { get; set; } = "Free";
    public string Status { get; set; } = "active";
    public DateTime? CurrentPeriodStart { get; set; }
    public DateTime? CurrentPeriodEnd { get; set; }
    public bool AutoRenew { get; set; }
    public string? Provider { get; set; }
    public string? ExternalSubscriptionId { get; set; }

    public UsageDto Usage { get; set; } = new();
    public PlanLimitsDto Limits { get; set; } = new();
}

public class UsageDto
{
    public int MessagesUsed { get; set; }
    public int MessagesLimit { get; set; }
    public int FileUploadsUsed { get; set; }
    public int FileUploadsLimit { get; set; }
    public int TokensUsed { get; set; }
    public int TokensLimit { get; set; }
    public DateTime PeriodStart { get; set; }
    public DateTime PeriodEnd { get; set; }
}

public class ChangePlanRequest
{
    public string PlanCode { get; set; } = string.Empty;
    public string? Provider { get; set; }
    public string? ExternalId { get; set; }
    public DateTime? PeriodEnd { get; set; }
}