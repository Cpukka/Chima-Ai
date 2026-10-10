using Chima.Application.DTOs.Admin;

namespace Chima.Application.Interfaces.Admin;

public interface IAdminMetricsService
{
    Task<DashboardMetricsDto> GetDashboardAsync(int days, CancellationToken ct);
    Task<RevenueMetricsDto> GetRevenueAsync(int days, CancellationToken ct);
    Task<AiCostMetricsDto> GetAiCostsAsync(int days, CancellationToken ct);
}