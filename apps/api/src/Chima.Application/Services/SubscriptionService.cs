using Chima.Application.Common;
using Chima.Application.DTOs.Subscription;
using Chima.Application.Interfaces;
using Chima.Domain.Entities;
using Chima.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Chima.Application.Services;

public class SubscriptionService : ISubscriptionService
{
    private readonly ISubscriptionRepository _subs;
    private readonly IUsageRepository _usage;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(
        ISubscriptionRepository subs,
        IUsageRepository usage,
        IUnitOfWork uow,
        ILogger<SubscriptionService> logger)
    {
        _subs = subs;
        _usage = usage;
        _uow = uow;
        _logger = logger;
    }

    public async Task<IReadOnlyList<PlanDto>> GetPlansAsync(bool includeInactive, CancellationToken ct)
    {
        var plans = await _subs.GetAllPlansAsync(includeInactive, ct);
        return plans.Select(ToPlanDto).ToList();
    }

    public async Task<SubscriptionStatusDto> GetCurrentAsync(Guid userId, CancellationToken ct)
    {
        var (plan, subscription, isDefaultFallback) = await ResolveEffectivePlanAsync(userId, ct);
        var (periodStart, periodEnd) = CurrentMonth();

        var (msgs, files, tokens, _) = await _usage.GetMonthlyUsageAsync(userId, periodStart, periodEnd, ct);
        var limits = plan.UsageLimit;

        return new SubscriptionStatusDto
        {
            PlanCode = plan.Code,
            PlanName = plan.DisplayName,
            Status = subscription?.Status.ToString().ToLowerInvariant() ?? "active",
            CurrentPeriodStart = subscription?.CurrentPeriodStart ?? periodStart,
            CurrentPeriodEnd = subscription?.CurrentPeriodEnd,
            AutoRenew = subscription?.AutoRenew ?? true,
            Provider = subscription?.Provider.ToString(),
            ExternalSubscriptionId = subscription?.ExternalSubscriptionId,
            Usage = new UsageDto
            {
                MessagesUsed = msgs,
                MessagesLimit = limits?.MonthlyMessages ?? 0,
                FileUploadsUsed = files,
                FileUploadsLimit = limits?.MonthlyFileUploads ?? 0,
                TokensUsed = tokens,
                TokensLimit = limits?.MonthlyTokens ?? 0,
                PeriodStart = periodStart,
                PeriodEnd = periodEnd
            },
            Limits = ToLimitsDto(limits)
        };
    }

    public async Task<UsageCheckResult> CheckQuotaAsync(Guid userId, string feature, CancellationToken ct)
    {
        var (plan, _, _) = await ResolveEffectivePlanAsync(userId, ct);
        var limits = plan.UsageLimit;

        var (periodStart, periodEnd) = CurrentMonth();
        var (msgs, files, tokens, _) = await _usage.GetMonthlyUsageAsync(userId, periodStart, periodEnd, ct);

        int used, limit;
        switch (feature.ToLowerInvariant())
        {
            case "message":
            case "ai_message":
                used = msgs;
                limit = limits?.MonthlyMessages ?? 0;
                break;
            case "file_upload":
                used = files;
                limit = limits?.MonthlyFileUploads ?? 0;
                break;
            case "tokens":
                used = tokens;
                limit = limits?.MonthlyTokens ?? 0;
                break;
            default:
                return new UsageCheckResult
                {
                    Allowed = true,
                    Feature = feature,
                    Used = 0,
                    Limit = -1,
                    Remaining = -1,
                    ResetAt = periodEnd
                };
        }

        // -1 = unlimited
        var unlimited = limit < 0;
        var allowed = unlimited || used < limit;
        var remaining = unlimited ? -1 : Math.Max(0, limit - used);

        return new UsageCheckResult
        {
            Allowed = allowed,
            Feature = feature,
            Used = used,
            Limit = limit,
            Remaining = remaining,
            ResetAt = periodEnd,
            BlockReason = allowed
                ? null
                : $"You've reached your monthly {feature.Replace('_', ' ')} limit on the {plan.DisplayName} plan. Upgrade to continue."
        };
    }

    public async Task RecordUsageAsync(Guid userId, string feature, int amount, CancellationToken ct)
    {
        // This is handled implicitly via usage_records inserts by AIService.
        // We only need to log here for future feature-specific counters.
        _logger.LogDebug("Usage event: {Feature} x{Amount} for user {UserId}", feature, amount, userId);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<SubscriptionStatusDto> ChangePlanAsync(
        Guid userId, string planCode, string provider, string? externalId, DateTime? periodEnd, CancellationToken ct)
    {
        var plan = await _subs.GetPlanByCodeAsync(planCode, ct)
            ?? throw new NotFoundException($"Plan '{planCode}' not found.");

        // Deactivate existing active subscriptions
        var current = await _subs.GetActiveSubscriptionAsync(userId, ct);
        if (current is not null)
        {
            current.Status = SubscriptionStatus.Expired;
            current.EndedAt = DateTime.UtcNow;
        }

        var start = DateTime.UtcNow;
        var end = periodEnd ?? start.AddMonths(1);

        var sub = new UserSubscription
        {
            UserId = userId,
            PlanId = plan.Id,
            Status = SubscriptionStatus.Active,
            CurrentPeriodStart = start,
            CurrentPeriodEnd = end,
            Provider = ParseProvider(provider),
            ExternalSubscriptionId = externalId,
            AutoRenew = true
        };
        await _subs.AddSubscriptionAsync(sub, ct);

        var payment = new Payment
        {
            UserId = userId,
            SubscriptionId = sub.Id,
            Provider = sub.Provider,
            ExternalPaymentId = externalId,
            Amount = plan.MonthlyPrice,
            Currency = plan.Currency,
            Status = PaymentStatus.Succeeded,
            PaidAt = DateTime.UtcNow
        };
        await _subs.AddPaymentAsync(payment, ct);

        await _uow.SaveChangesAsync(ct);
        _logger.LogInformation("User {UserId} changed to plan {Plan} via {Provider}.", userId, planCode, provider);

        return await GetCurrentAsync(userId, ct);
    }

    public async Task<SubscriptionStatusDto> CancelAsync(Guid userId, CancellationToken ct)
    {
        var sub = await _subs.GetActiveSubscriptionAsync(userId, ct)
            ?? throw new NotFoundException("No active subscription.");

        sub.Status = SubscriptionStatus.Cancelled;
        sub.AutoRenew = false;
        sub.CancelledAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("User {UserId} cancelled subscription.", userId);
        return await GetCurrentAsync(userId, ct);
    }

    public async Task<SubscriptionStatusDto> RestoreAsync(Guid userId, CancellationToken ct)
    {
        var sub = await _subs.GetActiveSubscriptionAsync(userId, ct)
            ?? throw new NotFoundException("No subscription to restore.");

        sub.Status = SubscriptionStatus.Active;
        sub.AutoRenew = true;
        sub.CancelledAt = null;
        await _uow.SaveChangesAsync(ct);

        return await GetCurrentAsync(userId, ct);
    }

    // ---------- helpers ----------

    private async Task<(SubscriptionPlan Plan, UserSubscription? Subscription, bool IsFallback)> ResolveEffectivePlanAsync(
        Guid userId, CancellationToken ct)
    {
        var sub = await _subs.GetActiveSubscriptionAsync(userId, ct);
        if (sub is not null)
        {
            // If subscription ended/cancelled but still in period, still applies
            if (sub.Status == SubscriptionStatus.Active
                || sub.Status == SubscriptionStatus.Trialing
                || sub.Status == SubscriptionStatus.PastDue
                || (sub.Status == SubscriptionStatus.Cancelled && sub.CurrentPeriodEnd > DateTime.UtcNow))
            {
                var plan = await _subs.GetPlanByIdAsync(sub.PlanId, ct);
                if (plan is not null) return (plan, sub, false);
            }
        }

        var fallback = await _subs.GetDefaultPlanAsync(ct)
            ?? throw new AppException("no_plan", "No default plan configured.", 500);
        return (fallback, null, true);
    }

    private static (DateTime start, DateTime end) CurrentMonth()
    {
        var now = DateTime.UtcNow;
        var start = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var end = start.AddMonths(1);
        return (start, end);
    }

    private static PaymentProvider ParseProvider(string provider) =>
        provider.ToLowerInvariant() switch
        {
            "google_play" => PaymentProvider.GooglePlay,
            "stripe" => PaymentProvider.Stripe,
            "paystack" => PaymentProvider.Paystack,
            _ => PaymentProvider.Manual
        };

    private static PlanDto ToPlanDto(SubscriptionPlan p) => new()
    {
        Id = p.Id,
        Code = p.Code,
        DisplayName = p.DisplayName,
        Description = p.Description,
        MonthlyPrice = p.MonthlyPrice,
        YearlyPrice = p.YearlyPrice,
        Currency = p.Currency,
        SortOrder = p.SortOrder,
        IsDefault = p.IsDefault,
        Limits = ToLimitsDto(p.UsageLimit)
    };

    private static PlanLimitsDto ToLimitsDto(UsageLimit? l) => new()
    {
        MonthlyMessages = l?.MonthlyMessages ?? 0,
        MonthlyFileUploads = l?.MonthlyFileUploads ?? 0,
        MonthlyTokens = l?.MonthlyTokens ?? 0,
        MaxFileSizeMb = l?.MaxFileSizeMb ?? 0,
        MaxConversations = l?.MaxConversations ?? 0,
        MonthlyCreditAllowance = l?.MonthlyCreditAllowance ?? 0
    };
}