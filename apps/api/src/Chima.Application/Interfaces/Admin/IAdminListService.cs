using Chima.Application.DTOs.Admin;
using Chima.Application.Services.Admin;

namespace Chima.Application.Interfaces.Admin;

public interface IAdminListService
{
    Task<PagedAdminResult<AdminPaymentListItemDto>> ListPaymentsAsync(AdminListQuery query, CancellationToken ct);
    Task<PagedAdminResult<AdminSubscriptionListItemDto>> ListSubscriptionsAsync(AdminListQuery query, CancellationToken ct);
    Task<List<AdminPlanOptionDto>> ListPlansAsync(CancellationToken ct);
}