using Chima.Domain.Entities;

namespace Chima.Application.Interfaces.Admin;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog log, CancellationToken ct);
    Task<(List<AuditLog> Items, int Total)> ListAsync(
        int page, int pageSize, string? action, Guid? actorUserId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}