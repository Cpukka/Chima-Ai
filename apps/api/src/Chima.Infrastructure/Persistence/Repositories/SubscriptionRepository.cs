using Chima.Application.Interfaces;
using Chima.Domain.Entities;
using Chima.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Persistence.Repositories;

public class SubscriptionRepository : ISubscriptionRepository
{
    private readonly AppDbContext _db;
    public SubscriptionRepository(AppDbContext db) => _db = db;

    public Task<SubscriptionPlan?> GetPlanByCodeAsync(string code, CancellationToken ct) =>
        _db.SubscriptionPlans.Include(p => p.UsageLimit)
            .FirstOrDefaultAsync(p => p.Code == code && p.IsActive, ct);

    public Task<SubscriptionPlan?> GetPlanByIdAsync(Guid id, CancellationToken ct) =>
        _db.SubscriptionPlans.Include(p => p.UsageLimit)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<SubscriptionPlan?> GetDefaultPlanAsync(CancellationToken ct) =>
        _db.SubscriptionPlans.Include(p => p.UsageLimit)
            .Where(p => p.IsActive && p.IsDefault)
            .FirstOrDefaultAsync(ct)
        ?? _db.SubscriptionPlans.Include(p => p.UsageLimit)
            .Where(p => p.IsActive)
            .OrderBy(p => p.SortOrder)
            .FirstOrDefaultAsync(ct);

    public Task<List<SubscriptionPlan>> GetAllPlansAsync(bool includeInactive, CancellationToken ct)
    {
        var q = _db.SubscriptionPlans.Include(p => p.UsageLimit).AsQueryable();
        if (!includeInactive) q = q.Where(p => p.IsActive);
        return q.OrderBy(p => p.SortOrder).ToListAsync(ct);
    }

    public Task<UserSubscription?> GetActiveSubscriptionAsync(Guid userId, CancellationToken ct) =>
        _db.UserSubscriptions
            .Include(s => s.Plan).ThenInclude(p => p.UsageLimit)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync(ct);

    public Task<List<UserSubscription>> GetSubscriptionHistoryAsync(Guid userId, CancellationToken ct) =>
        _db.UserSubscriptions
            .Include(s => s.Plan)
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(ct);

    public async Task AddSubscriptionAsync(UserSubscription subscription, CancellationToken ct) =>
        await _db.UserSubscriptions.AddAsync(subscription, ct);

    public async Task AddPaymentAsync(Payment payment, CancellationToken ct) =>
        await _db.Payments.AddAsync(payment, ct);

    public Task<List<Payment>> GetPaymentHistoryAsync(Guid userId, CancellationToken ct) =>
        _db.Payments.Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .Take(50)
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}