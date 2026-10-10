using Chima.Application.Interfaces;
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Persistence.Repositories;

public class MessageRepository : IMessageRepository
{
    private const int MaxLimit = 100;
    private readonly AppDbContext _db;
    public MessageRepository(AppDbContext db) => _db = db;

    public async Task<Message?> GetOwnedAsync(Guid id, Guid userId, CancellationToken ct)
    {
        return await _db.Messages
            .Where(m => m.Id == id && m.Conversation.UserId == userId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<(List<Message> Items, string? NextCursor, bool HasMore)> ListByConversationAsync(
        Guid conversationId, string? cursor, int limit, bool ascending, CancellationToken ct)
    {
        limit = Math.Clamp(limit, 1, MaxLimit);
        var q = _db.Messages.Where(m => m.ConversationId == conversationId);

        if (!string.IsNullOrEmpty(cursor) && DateTime.TryParse(cursor, out var cursorTime))
            q = ascending
                ? q.Where(m => m.CreatedAt > cursorTime)
                : q.Where(m => m.CreatedAt < cursorTime);

        var items = ascending
            ? await q.OrderBy(m => m.CreatedAt).Take(limit + 1).ToListAsync(ct)
            : await q.OrderByDescending(m => m.CreatedAt).Take(limit + 1).ToListAsync(ct);

        var hasMore = items.Count > limit;
        if (hasMore) items = items.Take(limit).ToList();
        if (!ascending) items.Reverse();

        var next = hasMore ? items.Last().CreatedAt.ToString("O") : null;
        return (items, next, hasMore);
    }

    public Task<Message?> GetLatestByConversationAsync(Guid conversationId, CancellationToken ct) =>
        _db.Messages
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.CreatedAt)
            .FirstOrDefaultAsync(ct);

    public async Task AddAsync(Message message, CancellationToken ct) =>
        await _db.Messages.AddAsync(message, ct);

    public async Task AddRangeAsync(IEnumerable<Message> messages, CancellationToken ct) =>
        await _db.Messages.AddRangeAsync(messages, ct);

    public Task DeleteAsync(Message message, CancellationToken ct)
    {
        _db.Messages.Remove(message);
        return Task.CompletedTask;
    }

    public async Task DeleteDescendantsAsync(Guid messageId, CancellationToken ct)
    {
        var descendants = await _db.Messages
            .Where(m => m.ParentMessageId == messageId || m.BranchRootId == messageId)
            .ToListAsync(ct);
        _db.Messages.RemoveRange(descendants);
    }

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}