using Chima.Application.DTOs.Subscription;

namespace Chima.Application.Interfaces;

public interface ISubscriptionService
{
    Task<SubscriptionStatusDto> GetCurrentAsync(Guid userId, CancellationToken ct);
    Task<IReadOnlyList<PlanDto>> GetPlansAsync(bool includeInactive, CancellationToken ct);
    Task<UsageCheckResult> CheckQuotaAsync(Guid userId, string feature, CancellationToken ct);
    Task RecordUsageAsync(Guid userId, string feature, int amount, CancellationToken ct);

    /// <summary>Used by Phase 9b/9c webhooks and by admin tools for testing.</summary>
    Task<SubscriptionStatusDto> ChangePlanAsync(
        Guid userId, string planCode, string provider, string? externalId, DateTime? periodEnd, CancellationToken ct);

    Task<SubscriptionStatusDto> CancelAsync(Guid userId, CancellationToken ct);
    Task<SubscriptionStatusDto> RestoreAsync(Guid userId, CancellationToken ct);
}

public class UsageCheckResult
{
    public bool Allowed { get; init; }
    public string Feature { get; init; } = string.Empty;
    public int Used { get; init; }
    public int Limit { get; init; } = -1;
    public int Remaining { get; init; } = -1;
    public DateTime ResetAt { get; init; }
    public string? BlockReason { get; init; }
}