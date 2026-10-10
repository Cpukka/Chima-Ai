using Chima.Application.Interfaces.Admin;
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Persistence.Repositories;

public class AuditLogRepository : IAuditLogRepository
{
    private readonly AppDbContext _db;
    public AuditLogRepository(AppDbContext db) => _db = db;

    public async Task AddAsync(AuditLog log, CancellationToken ct) =>
        await _db.AuditLogs.AddAsync(log, ct);

    public async Task<(List<AuditLog> Items, int Total)> ListAsync(
        int page, int pageSize, string? action, Guid? actorUserId, CancellationToken ct)
    {
        var q = _db.AuditLogs.AsQueryable();
        if (!string.IsNullOrWhiteSpace(action)) q = q.Where(l => l.Action == action);
        if (actorUserId.HasValue) q = q.Where(l => l.ActorUserId == actorUserId.Value);

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}