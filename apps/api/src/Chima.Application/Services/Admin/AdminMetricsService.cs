using Chima.Application.DTOs.Admin;
using Chima.Application.Interfaces.Admin;

namespace Chima.Application.Services.Admin;

public class AdminMetricsService : IAdminMetricsService
{
    private readonly IAdminMetricsRepository _repo;
    public AdminMetricsService(IAdminMetricsRepository repo) => _repo = repo;

    public Task<DashboardMetricsDto> GetDashboardAsync(int days, CancellationToken ct) =>
        _repo.GetDashboardAsync(days, ct);

    public Task<RevenueMetricsDto> GetRevenueAsync(int days, CancellationToken ct) =>
        _repo.GetRevenueAsync(days, ct);

    public Task<AiCostMetricsDto> GetAiCostsAsync(int days, CancellationToken ct) =>
        _repo.GetAiCostsAsync(days, ct);
}