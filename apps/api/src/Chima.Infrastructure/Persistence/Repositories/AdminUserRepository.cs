using Chima.Application.Common;
using Chima.Application.DTOs.Admin;
using Chima.Application.Interfaces.Admin;
using Chima.Infrastructure.Persistence;
using Chima.Domain.Entities;
using Chima.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Persistence.Repositories;

public class AdminUserRepository : IAdminUserRepository
{
    private readonly AppDbContext _db;
    public AdminUserRepository(AppDbContext db) => _db = db;

    public async Task<PagedAdminResult<AdminUserListItemDto>> ListAsync(
        int page, int pageSize, string? search, string? status, string? planCode, CancellationToken ct)
    {
        var q = from u in _db.Users
                join p in _db.UserProfiles on u.Id equals p.UserId into profiles
                from prof in profiles.DefaultIfEmpty()
                select new { User = u, Profile = prof };

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = $"%{search.Trim()}%";
            q = q.Where(x => EF.Functions.ILike(x.User.Email, s)
                          || (x.Profile != null && EF.Functions.ILike(x.Profile.DisplayName!, s)));
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<UserStatus>(status, true, out var st))
            q = q.Where(x => x.User.Status == st);

        var total = await q.CountAsync(ct);

        var users = await q.OrderByDescending(x => x.User.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new { x.User, x.Profile })
            .ToListAsync(ct);

        var items = new List<AdminUserListItemDto>(users.Count);
        foreach (var u in users)
        {
            var plan = await GetActivePlanCodeAsync(u.User.Id, ct);
            var admin = await _db.AdminUsers.FirstOrDefaultAsync(a => a.UserId == u.User.Id && a.IsActive, ct);

            var messageCount = await _db.UsageRecords.CountAsync(r => r.UserId == u.User.Id && r.Success, ct);
            var spent = await _db.Payments.Where(p => p.UserId == u.User.Id && p.Status == PaymentStatus.Succeeded)
                .SumAsync(p => (decimal?)p.Amount, ct) ?? 0m;

            items.Add(new AdminUserListItemDto
            {
                Id = u.User.Id,
                Email = u.User.Email,
                DisplayName = u.Profile?.DisplayName,
                EmailVerified = u.User.EmailVerified,
                Status = u.User.Status.ToString().ToLowerInvariant(),
                PlanCode = plan.Code,
                PlanName = plan.Name,
                CreatedAt = u.User.CreatedAt,
                LastLoginAt = u.User.LastLoginAt,
                IsAdmin = admin is not null,
                TotalMessages = messageCount,
                TotalSpent = spent
            });
        }

        return new PagedAdminResult<AdminUserListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    public async Task<AdminUserDetailDto> GetDetailAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new NotFoundException("User not found.");

        var admin = await _db.AdminUsers.FirstOrDefaultAsync(a => a.UserId == userId && a.IsActive, ct);
        var plan = await GetActivePlanCodeAsync(userId, ct);

        var sub = await _db.UserSubscriptions
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt).FirstOrDefaultAsync(ct);

        var (periodStart, periodEnd) = CurrentMonth();
        var records = await _db.UsageRecords
            .Where(r => r.UserId == userId && r.CreatedAt >= periodStart && r.CreatedAt < periodEnd)
            .ToListAsync(ct);

        var payments = await _db.Payments
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt).Take(20).ToListAsync(ct);

        var convCount = await _db.Conversations.CountAsync(c => c.UserId == userId && c.DeletedAt == null, ct);
        var msgCount = await _db.Messages.CountAsync(m => m.Conversation.UserId == userId, ct);
        var fileCount = await _db.UploadedFiles.CountAsync(f => f.UserId == userId, ct);

        var limits = await _db.UsageLimits.FirstOrDefaultAsync(l => l.PlanId == plan.Id, ct);

        return new AdminUserDetailDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.Profile?.DisplayName,
            AvatarUrl = user.Profile?.AvatarUrl,
            EmailVerified = user.EmailVerified,
            Status = user.Status.ToString().ToLowerInvariant(),
            IsAdmin = admin is not null,
            AdminRole = admin?.Role.ToString(),
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            PlanCode = plan.Code,
            PlanName = plan.Name,
            SubscriptionStatus = sub?.Status.ToString().ToLowerInvariant() ?? "active",
            CurrentPeriodEnd = sub?.CurrentPeriodEnd,
            AutoRenew = sub?.AutoRenew ?? true,
            MessagesUsedThisMonth = records.Count(r => r.Success),
            MessagesLimit = limits?.MonthlyMessages ?? 0,
            FileUploadsUsedThisMonth = await _db.UploadedFiles
                .CountAsync(f => f.UserId == userId && f.CreatedAt >= periodStart && f.CreatedAt < periodEnd, ct),
            FileUploadsLimit = limits?.MonthlyFileUploads ?? 0,
            TokensUsedThisMonth = records.Sum(r => r.TotalTokens),
            TotalSpent = payments.Where(p => p.Status == PaymentStatus.Succeeded).Sum(p => p.Amount),
            ConversationCount = convCount,
            MessageCount = msgCount,
            FileCount = fileCount,
            RecentPayments = payments.Select(p => new AdminPaymentDto
            {
                Id = p.Id,
                Amount = p.Amount,
                Currency = p.Currency,
                Status = p.Status.ToString().ToLowerInvariant(),
                Provider = p.Provider.ToString(),
                ExternalPaymentId = p.ExternalPaymentId,
                CreatedAt = p.CreatedAt,
                PaidAt = p.PaidAt
            }).ToList()
        };
    }

    public Task<User?> GetUserAsync(Guid userId, CancellationToken ct) =>
        _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct);

    // ---------- helpers ----------

    private async Task<(Guid Id, string Code, string Name)> GetActivePlanCodeAsync(Guid userId, CancellationToken ct)
    {
        var sub = await _db.UserSubscriptions
            .Include(s => s.Plan)
            .Where(s => s.UserId == userId
                     && (s.Status == SubscriptionStatus.Active
                      || s.Status == SubscriptionStatus.Trialing
                      || s.Status == SubscriptionStatus.PastDue
                      || (s.Status == SubscriptionStatus.Cancelled && s.CurrentPeriodEnd > DateTime.UtcNow)))
            .OrderByDescending(s => s.CreatedAt).FirstOrDefaultAsync(ct);

        if (sub?.Plan is not null)
            return (sub.Plan.Id, sub.Plan.Code, sub.Plan.DisplayName);

        var def = await _db.SubscriptionPlans
            .Where(p => p.IsActive && p.IsDefault)
            .FirstOrDefaultAsync(ct)
            ?? await _db.SubscriptionPlans.Where(p => p.IsActive).OrderBy(p => p.SortOrder).FirstAsync(ct);

        return (def.Id, def.Code, def.DisplayName);
    }

    private static (DateTime start, DateTime end) CurrentMonth()
    {
        var now = DateTime.UtcNow;
        var start = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        return (start, start.AddMonths(1));
    }
}