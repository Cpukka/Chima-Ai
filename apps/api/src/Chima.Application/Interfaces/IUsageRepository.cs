using Chima.Domain.Entities;

namespace Chima.Application.Interfaces;

public interface IUsageRepository
{
    /// <summary>Returns usage for the current calendar month for the given user.</summary>
    Task<(int MessageCount, int FileUploadCount, int TokenCount, decimal Cost)> GetMonthlyUsageAsync(
        Guid userId, DateTime periodStart, DateTime periodEnd, CancellationToken ct);

    Task<int> GetMonthlyAiRequestCountAsync(Guid userId, DateTime periodStart, DateTime periodEnd, CancellationToken ct);
}