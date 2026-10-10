using Chima.Domain.Entities;

namespace Chima.Application.Interfaces;

public interface IUsageRecordRepository
{
    Task AddAsync(UsageRecord record, CancellationToken ct);
}