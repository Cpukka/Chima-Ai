using Chima.Application.DTOs.Admin;
using Chima.Application.Interfaces.Admin;

namespace Chima.Application.Services.Admin;

public class AdminListService : IAdminListService
{
    private readonly IAdminPaymentRepository _payments;
    private readonly IAdminSubscriptionRepository _subs;
    private readonly IAdminPlanRepository _plans;

    public AdminListService(
        IAdminPaymentRepository payments,
        IAdminSubscriptionRepository subs,
        IAdminPlanRepository plans)
    {
        _payments = payments;
        _subs = subs;
        _plans = plans;
    }

    public Task<PagedAdminResult<AdminPaymentListItemDto>> ListPaymentsAsync(AdminListQuery query, CancellationToken ct) =>
        _payments.ListAsync(query, ct);

    public Task<PagedAdminResult<AdminSubscriptionListItemDto>> ListSubscriptionsAsync(AdminListQuery query, CancellationToken ct) =>
        _subs.ListAsync(query, ct);

    public Task<List<AdminPlanOptionDto>> ListPlansAsync(CancellationToken ct) =>
        _plans.ListAsync(ct);
}