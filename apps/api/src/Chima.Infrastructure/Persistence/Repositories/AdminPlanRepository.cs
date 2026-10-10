using Chima.Application.DTOs.Admin;
using Chima.Application.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Persistence.Repositories;

public class AdminPlanRepository : IAdminPlanRepository
{
    private readonly AppDbContext _db;
    public AdminPlanRepository(AppDbContext db) => _db = db;

    public Task<List<AdminPlanOptionDto>> ListAsync(CancellationToken ct) =>
        _db.SubscriptionPlans
            .OrderBy(p => p.SortOrder)
            .Select(p => new AdminPlanOptionDto
            {
                Id = p.Id,
                Code = p.Code,
                DisplayName = p.DisplayName,
                MonthlyPrice = p.MonthlyPrice,
                YearlyPrice = p.YearlyPrice,
                Currency = p.Currency,
                IsActive = p.IsActive,
                IsDefault = p.IsDefault,
                SortOrder = p.SortOrder
            })
            .ToListAsync(ct);
}