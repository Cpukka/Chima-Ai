using Chima.Application.DTOs.Admin;
using Chima.Application.Interfaces.Admin;
using Chima.Application.Services.Admin;
using Chima.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Persistence.Repositories;

public class AdminPaymentRepository : IAdminPaymentRepository
{
    private readonly AppDbContext _db;
    public AdminPaymentRepository(AppDbContext db) => _db = db;

    public async Task<PagedAdminResult<AdminPaymentListItemDto>> ListAsync(AdminListQuery query, CancellationToken ct)
    {
        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        var q = from p in _db.Payments
                join u in _db.Users on p.UserId equals u.Id
                join prof in _db.UserProfiles on u.Id equals prof.UserId into profs
                from prof in profs.DefaultIfEmpty()
                select new { Payment = p, User = u, Profile = prof };

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var s = $"%{query.Search.Trim()}%";
            q = q.Where(x => EF.Functions.ILike(x.User.Email, s)
                          || (x.Profile != null && EF.Functions.ILike(x.Profile.DisplayName!, s))
                          || (x.Payment.ExternalPaymentId != null && EF.Functions.ILike(x.Payment.ExternalPaymentId, s)));
        }

        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<PaymentStatus>(query.Status, true, out var st))
            q = q.Where(x => x.Payment.Status == st);

        if (!string.IsNullOrWhiteSpace(query.Provider) && Enum.TryParse<PaymentProvider>(query.Provider, true, out var pr))
            q = q.Where(x => x.Payment.Provider == pr);

        if (query.From.HasValue)
            q = q.Where(x => x.Payment.CreatedAt >= query.From.Value);
        if (query.To.HasValue)
            q = q.Where(x => x.Payment.CreatedAt <= query.To.Value);

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(x => x.Payment.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new AdminPaymentListItemDto
            {
                Id = x.Payment.Id,
                UserId = x.User.Id,
                UserEmail = x.User.Email,
                UserDisplayName = x.Profile != null ? x.Profile.DisplayName : null,
                Amount = x.Payment.Amount,
                Currency = x.Payment.Currency,
                Status = x.Payment.Status.ToString().ToLowerInvariant(),
                Provider = x.Payment.Provider.ToString(),
                ExternalPaymentId = x.Payment.ExternalPaymentId,
                SubscriptionPlanCode = _db.UserSubscriptions
                    .Where(s => s.Id == x.Payment.SubscriptionId)
                    .Select(s => s.Plan.Code)
                    .FirstOrDefault(),
                CreatedAt = x.Payment.CreatedAt,
                PaidAt = x.Payment.PaidAt,
                RefundedAt = x.Payment.RefundedAt
            })
            .ToListAsync(ct);

        return new PagedAdminResult<AdminPaymentListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }
}