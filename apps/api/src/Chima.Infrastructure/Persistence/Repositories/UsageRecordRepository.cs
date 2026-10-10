using Chima.Application.Interfaces;
using Chima.Domain.Entities;

namespace Chima.Infrastructure.Persistence.Repositories;

public class UsageRecordRepository : IUsageRecordRepository
{
    private readonly AppDbContext _db;
    public UsageRecordRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(UsageRecord record, CancellationToken ct) =>
        await _db.UsageRecords.AddAsync(record, ct);
}