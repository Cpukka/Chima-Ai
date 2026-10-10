using Chima.Application.Interfaces;
using Chima.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Persistence.Repositories;

public class UsageRepository : IUsageRepository
{
    private readonly AppDbContext _db;
    public UsageRepository(AppDbContext db) => _db = db;

    public async Task<(int MessageCount, int FileUploadCount, int TokenCount, decimal Cost)> GetMonthlyUsageAsync(
        Guid userId, DateTime periodStart, DateTime periodEnd, CancellationToken ct)
    {
        var records = await _db.UsageRecords
            .Where(r => r.UserId == userId && r.CreatedAt >= periodStart && r.CreatedAt < periodEnd)
            .ToListAsync(ct);

        // Count only successful AI messages (one per assistant turn)
        var messageCount = records.Count(r => r.Success);

        // File uploads are tracked separately in uploaded_files
        var fileUploadCount = await _db.UploadedFiles
            .CountAsync(f => f.UserId == userId
                          && f.CreatedAt >= periodStart
                          && f.CreatedAt < periodEnd
                          && f.ProcessingStatus != FileProcessingStatus.Failed, ct);

        var tokens = records.Sum(r => r.TotalTokens);
        var cost = records.Sum(r => r.EstimatedCost);

        return (messageCount, fileUploadCount, tokens, cost);
    }

    public Task<int> GetMonthlyAiRequestCountAsync(Guid userId, DateTime periodStart, DateTime periodEnd, CancellationToken ct) =>
        _db.UsageRecords.CountAsync(
            r => r.UserId == userId
              && r.CreatedAt >= periodStart
              && r.CreatedAt < periodEnd
              && r.Success, ct);
}