using Chima.Application.Interfaces;
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Persistence.Repositories;

public class ConversationRepository : IConversationRepository
{
    private const int MaxLimit = 100;
    private readonly AppDbContext _db;
    public ConversationRepository(AppDbContext db) => _db = db;

    public Task<Conversation?> GetOwnedAsync(Guid id, Guid userId, CancellationToken ct, bool includeMessages = false)
    {
        var q = _db.Conversations.AsQueryable();
        if (includeMessages) q = q.Include(c => c.Messages);
        return q.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId, ct);
    }

    public async Task<(List<Conversation> Items, string? NextCursor, bool HasMore)> ListAsync(
        Guid userId, string? cursor, int limit, bool includeArchived, CancellationToken ct)
    {
        limit = Math.Clamp(limit, 1, MaxLimit);
        var q = _db.Conversations
            .Where(c => c.UserId == userId && c.DeletedAt == null);
        if (!includeArchived) q = q.Where(c => !c.IsArchived);

        if (!string.IsNullOrEmpty(cursor) && DateTime.TryParse(cursor, out var cursorTime))
            q = q.Where(c => c.UpdatedAt < cursorTime);

        var items = await q
            .OrderByDescending(c => c.IsPinned)
            .ThenByDescending(c => c.UpdatedAt)
            .Take(limit + 1)
            .ToListAsync(ct);

        var hasMore = items.Count > limit;
        if (hasMore) items = items.Take(limit).ToList();
        var next = hasMore ? items.Last().UpdatedAt.ToString("O") : null;

        return (items, next, hasMore);
    }

    public async Task<(List<Conversation> Items, string? NextCursor, bool HasMore)> SearchAsync(
        Guid userId, string query, string? cursor, int limit, CancellationToken ct)
    {
        limit = Math.Clamp(limit, 1, MaxLimit);
        var like = $"%{query}%";

        var q = _db.Conversations
            .Where(c => c.UserId == userId && c.DeletedAt == null)
            .Where(c => EF.Functions.ILike(c.Title, like)
                     || c.Messages.Any(m => EF.Functions.ILike(m.Content, like)));

        if (!string.IsNullOrEmpty(cursor) && DateTime.TryParse(cursor, out var cursorTime))
            q = q.Where(c => c.UpdatedAt < cursorTime);

        var items = await q
            .OrderByDescending(c => c.UpdatedAt)
            .Take(limit + 1)
            .ToListAsync(ct);

        var hasMore = items.Count > limit;
        if (hasMore) items = items.Take(limit).ToList();
        var next = hasMore ? items.Last().UpdatedAt.ToString("O") : null;

        return (items, next, hasMore);
    }

    public async Task AddAsync(Conversation conversation, CancellationToken ct) =>
        await _db.Conversations.AddAsync(conversation, ct);

    public Task<int> CountByUserAsync(Guid userId, CancellationToken ct) =>
        _db.Conversations.CountAsync(c => c.UserId == userId && c.DeletedAt == null, ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}