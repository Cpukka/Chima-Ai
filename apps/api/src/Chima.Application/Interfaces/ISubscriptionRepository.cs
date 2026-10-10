using Chima.Domain.Entities;

namespace Chima.Application.Interfaces;

public interface ISubscriptionRepository
{
    Task<SubscriptionPlan?> GetPlanByCodeAsync(string code, CancellationToken ct);
    Task<SubscriptionPlan?> GetPlanByIdAsync(Guid id, CancellationToken ct);
    Task<SubscriptionPlan?> GetDefaultPlanAsync(CancellationToken ct);
    Task<List<SubscriptionPlan>> GetAllPlansAsync(bool includeInactive, CancellationToken ct);

    Task<UserSubscription?> GetActiveSubscriptionAsync(Guid userId, CancellationToken ct);
    Task<List<UserSubscription>> GetSubscriptionHistoryAsync(Guid userId, CancellationToken ct);
    Task AddSubscriptionAsync(UserSubscription subscription, CancellationToken ct);
    Task AddPaymentAsync(Payment payment, CancellationToken ct);
    Task<List<Payment>> GetPaymentHistoryAsync(Guid userId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}