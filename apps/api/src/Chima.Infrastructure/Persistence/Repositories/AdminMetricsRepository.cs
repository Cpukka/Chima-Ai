using Chima.Application.DTOs.Admin;
using Chima.Application.Interfaces.Admin;
using Chima.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Persistence.Repositories;

public class AdminMetricsRepository : IAdminMetricsRepository
{
    private readonly AppDbContext _db;
    public AdminMetricsRepository(AppDbContext db) => _db = db;

    public async Task<DashboardMetricsDto> GetDashboardAsync(int days, CancellationToken ct)
    {
        var since = DateTime.UtcNow.AddDays(-days);

        var totalUsers = await _db.Users.CountAsync(ct);
        var newUsers = await _db.Users.CountAsync(u => u.CreatedAt >= since, ct);
        var activeUsers = await _db.Users.CountAsync(u => u.LastLoginAt >= since, ct);

        var paidUsers = await _db.UserSubscriptions
            .Where(s => s.Status == SubscriptionStatus.Active)
            .Select(s => s.UserId).Distinct().CountAsync(ct);

        var revenue = await _db.Payments
            .Where(p => p.Status == PaymentStatus.Succeeded && p.CreatedAt >= since)
            .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;

        var aiRequests = await _db.UsageRecords.CountAsync(r => r.CreatedAt >= since, ct);
        var aiCost = await _db.UsageRecords.Where(r => r.CreatedAt >= since)
            .SumAsync(r => (decimal?)r.EstimatedCost, ct) ?? 0m;
        var failedRequests = await _db.UsageRecords.CountAsync(r => r.CreatedAt >= since && !r.Success, ct);

        var totalConversations = await _db.Conversations.CountAsync(ct);
        var totalMessages = await _db.Messages.CountAsync(ct);
        var totalStorage = await _db.UploadedFiles.SumAsync(f => (long?)f.SizeBytes, ct) ?? 0L;

        // Timeseries â€” daily buckets
        var newUsersSeries = await _db.Users
            .Where(u => u.CreatedAt >= since)
            .GroupBy(u => u.CreatedAt.Date)
            .Select(g => new TimeSeriesPoint { Date = g.Key, Value = g.Count() })
            .OrderBy(p => p.Date).ToListAsync(ct);

        var revenueSeries = await _db.Payments
            .Where(p => p.Status == PaymentStatus.Succeeded && p.CreatedAt >= since)
            .GroupBy(p => p.CreatedAt.Date)
            .Select(g => new TimeSeriesPoint { Date = g.Key, Value = g.Sum(p => p.Amount) })
            .OrderBy(p => p.Date).ToListAsync(ct);

        var aiSeries = await _db.UsageRecords
            .Where(r => r.CreatedAt >= since)
            .GroupBy(r => r.CreatedAt.Date)
            .Select(g => new TimeSeriesPoint { Date = g.Key, Value = g.Count() })
            .OrderBy(p => p.Date).ToListAsync(ct);

        // MRR: sum of active plans' monthly prices
        var mrr = await (from s in _db.UserSubscriptions
                         join p in _db.SubscriptionPlans on s.PlanId equals p.Id
                         where s.Status == SubscriptionStatus.Active
                         select p.MonthlyPrice).SumAsync(ct);

        var grossMargin = revenue - aiCost;

        return new DashboardMetricsDto
        {
            TotalUsers = totalUsers,
            NewUsersInPeriod = newUsers,
            ActiveUsersInPeriod = activeUsers,
            PaidUsers = paidUsers,
            FreeUsers = totalUsers - paidUsers,
            RevenueInPeriod = revenue,
            MonthlyRecurringRevenue = mrr,
            AiRequestsInPeriod = aiRequests,
            EstimatedAiCostInPeriod = aiCost,
            GrossMarginInPeriod = grossMargin,
            GrossMarginPercent = revenue > 0 ? (double)(grossMargin / revenue) * 100 : 0,
            FailedRequestsInPeriod = failedRequests,
            TotalConversations = totalConversations,
            TotalMessages = totalMessages,
            TotalStorageBytes = totalStorage,
            NewUsersSeries = newUsersSeries,
            RevenueSeries = revenueSeries,
            AiRequestsSeries = aiSeries
        };
    }

    public async Task<RevenueMetricsDto> GetRevenueAsync(int days, CancellationToken ct)
    {
        var since = DateTime.UtcNow.AddDays(-days);

        var total = await _db.Payments.Where(p => p.Status == PaymentStatus.Succeeded)
            .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;
        var inPeriod = await _db.Payments.Where(p => p.Status == PaymentStatus.Succeeded && p.CreatedAt >= since)
            .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;

        var activeSubs = await _db.UserSubscriptions.CountAsync(s => s.Status == SubscriptionStatus.Active, ct);
        var cancelled = await _db.UserSubscriptions.CountAsync(s => s.CancelledAt >= since, ct);
        var newSubs = await _db.UserSubscriptions.CountAsync(s => s.CreatedAt >= since, ct);

        var mrr = await (from s in _db.UserSubscriptions
                         join p in _db.SubscriptionPlans on s.PlanId equals p.Id
                         where s.Status == SubscriptionStatus.Active
                         select p.MonthlyPrice).SumAsync(ct);

        var byPlan = await (from s in _db.UserSubscriptions
                            join p in _db.SubscriptionPlans on s.PlanId equals p.Id
                            where s.Status == SubscriptionStatus.Active
                            group p by new { p.Code, p.DisplayName, p.MonthlyPrice } into g
                            select new PlanRevenueDto
                            {
                                PlanCode = g.Key.Code,
                                PlanName = g.Key.DisplayName,
                                Subscribers = g.Count(),
                                Mrr = g.Sum(x => x.MonthlyPrice)
                            }).ToListAsync(ct);

        var churn = activeSubs + cancelled > 0 ? (double)cancelled / (activeSubs + cancelled) * 100 : 0;

        return new RevenueMetricsDto
        {
            TotalRevenue = total,
            RevenueInPeriod = inPeriod,
            Mrr = mrr,
            Arr = mrr * 12,
            ActiveSubscriptions = activeSubs,
            CancelledInPeriod = cancelled,
            NewSubscriptionsInPeriod = newSubs,
            ChurnPercent = churn,
            ByPlan = byPlan
        };
    }

    public async Task<AiCostMetricsDto> GetAiCostsAsync(int days, CancellationToken ct)
    {
        var since = DateTime.UtcNow.AddDays(-days);

        var records = await _db.UsageRecords.Where(r => r.CreatedAt >= since).ToListAsync(ct);

        var revenue = await _db.Payments
            .Where(p => p.Status == PaymentStatus.Succeeded && p.CreatedAt >= since)
            .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;

        var byModel = await (from r in _db.UsageRecords
                             join m in _db.AIModels on r.ModelId equals m.Id
                             join p in _db.AIProviders on m.ProviderId equals p.Id
                             where r.CreatedAt >= since
                             group new { r, m, p } by new { ModelId = m.Id, ModelName = m.Name, ProviderName = p.Name } into g
                             select new ModelCostDto
                             {
                                 ModelId = g.Key.ModelId,
                                 ModelName = g.Key.ModelName,
                                 ProviderName = g.Key.ProviderName,
                                 Requests = g.Count(),
                                 InputTokens = g.Sum(x => x.r.InputTokens),
                                 OutputTokens = g.Sum(x => x.r.OutputTokens),
                                 EstimatedCost = g.Sum(x => x.r.EstimatedCost)
                             }).ToListAsync(ct);

        var daily = await _db.UsageRecords
            .Where(r => r.CreatedAt >= since)
            .GroupBy(r => r.CreatedAt.Date)
            .Select(g => new TimeSeriesPoint { Date = g.Key, Value = g.Sum(x => x.EstimatedCost) })
            .OrderBy(p => p.Date).ToListAsync(ct);

        var cost = records.Sum(r => r.EstimatedCost);

        return new AiCostMetricsDto
        {
            RequestsInPeriod = records.Count,
            InputTokensInPeriod = records.Sum(r => r.InputTokens),
            OutputTokensInPeriod = records.Sum(r => r.OutputTokens),
            EstimatedCostInPeriod = cost,
            RevenueInPeriod = revenue,
            GrossMarginInPeriod = revenue - cost,
            GrossMarginPercent = revenue > 0 ? (double)((revenue - cost) / revenue) * 100 : 0,
            ByModel = byModel,
            DailyCost = daily
        };
    }
}