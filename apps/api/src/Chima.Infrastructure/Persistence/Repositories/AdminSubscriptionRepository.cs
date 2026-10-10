using Chima.Application.DTOs.Admin;
using Chima.Application.Interfaces.Admin;
using Chima.Application.Services.Admin;
using Chima.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Persistence.Repositories;

public class AdminSubscriptionRepository : IAdminSubscriptionRepository
{
    private readonly AppDbContext _db;
    public AdminSubscriptionRepository(AppDbContext db) => _db = db;

    public async Task<PagedAdminResult<AdminSubscriptionListItemDto>> ListAsync(AdminListQuery query, CancellationToken ct)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = from s in _db.UserSubscriptions
                join u in _db.Users on s.UserId equals u.Id
                join p in _db.SubscriptionPlans on s.PlanId equals p.Id
                join prof in _db.UserProfiles on u.Id equals prof.UserId into profs
                from prof in profs.DefaultIfEmpty()
                select new { Sub = s, User = u, Plan = p, Profile = prof };

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = $"%{query.Search.Trim()}%";
            q = q.Where(x => EF.Functions.ILike(x.User.Email, term)
                          || (x.Profile != null && EF.Functions.ILike(x.Profile.DisplayName!, term)));
        }

        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<SubscriptionStatus>(query.Status, true, out var st))
            q = q.Where(x => x.Sub.Status == st);

        if (!string.IsNullOrWhiteSpace(query.PlanCode))
            q = q.Where(x => x.Plan.Code == query.PlanCode);

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(x => x.Sub.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AdminSubscriptionListItemDto
            {
                Id = x.Sub.Id,
                UserId = x.User.Id,
                UserEmail = x.User.Email,
                UserDisplayName = x.Profile != null ? x.Profile.DisplayName : null,
                PlanCode = x.Plan.Code,
                PlanName = x.Plan.DisplayName,
                MonthlyPrice = x.Plan.MonthlyPrice,
                Status = x.Sub.Status.ToString().ToLowerInvariant(),
                Provider = x.Sub.Provider.ToString(),
                ExternalSubscriptionId = x.Sub.ExternalSubscriptionId,
                AutoRenew = x.Sub.AutoRenew,
                CurrentPeriodStart = x.Sub.CurrentPeriodStart,
                CurrentPeriodEnd = x.Sub.CurrentPeriodEnd,
                CancelledAt = x.Sub.CancelledAt,
                CreatedAt = x.Sub.CreatedAt
            })
            .ToListAsync(ct);

        return new PagedAdminResult<AdminSubscriptionListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }
}