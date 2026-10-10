using Chima.Application.DTOs.Admin;
using Chima.Application.Services.Admin;

namespace Chima.Application.Interfaces.Admin;

public interface IAdminPaymentRepository
{
    Task<PagedAdminResult<AdminPaymentListItemDto>> ListAsync(AdminListQuery query, CancellationToken ct);
}

public interface IAdminSubscriptionRepository
{
    Task<PagedAdminResult<AdminSubscriptionListItemDto>> ListAsync(AdminListQuery query, CancellationToken ct);
}

public interface IAdminPlanRepository
{
    Task<List<AdminPlanOptionDto>> ListAsync(CancellationToken ct);
}