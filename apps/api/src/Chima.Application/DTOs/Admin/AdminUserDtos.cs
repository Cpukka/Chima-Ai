namespace Chima.Application.DTOs.Admin;

public class AdminUserListItemDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public bool EmailVerified { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PlanCode { get; set; } = "free";
    public string PlanName { get; set; } = "Free";
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public bool IsAdmin { get; set; }
    public int TotalMessages { get; set; }
    public decimal TotalSpent { get; set; }
}

public class AdminUserDetailDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public bool EmailVerified { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public string? AdminRole { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }

    public string PlanCode { get; set; } = "free";
    public string PlanName { get; set; } = "Free";
    public string SubscriptionStatus { get; set; } = "active";
    public DateTime? CurrentPeriodEnd { get; set; }
    public bool AutoRenew { get; set; }

    public int MessagesUsedThisMonth { get; set; }
    public int MessagesLimit { get; set; }
    public int FileUploadsUsedThisMonth { get; set; }
    public int FileUploadsLimit { get; set; }
    public int TokensUsedThisMonth { get; set; }
    public decimal TotalSpent { get; set; }

    public int ConversationCount { get; set; }
    public int MessageCount { get; set; }
    public int FileCount { get; set; }

    public IReadOnlyList<AdminPaymentDto> RecentPayments { get; set; } = Array.Empty<AdminPaymentDto>();
}

public class AdminPaymentDto
{
    public Guid Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string Status { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string? ExternalPaymentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class SuspendUserRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class AdminChangePlanRequest
{
    public string PlanCode { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}