using Chima.Domain.Entities;

namespace Chima.Application.Interfaces;

public interface IMessageRepository
{
    Task<Message?> GetOwnedAsync(Guid id, Guid userId, CancellationToken ct);
    Task<(List<Message> Items, string? NextCursor, bool HasMore)> ListByConversationAsync(
        Guid conversationId, string? cursor, int limit, bool ascending, CancellationToken ct);
    Task<Message?> GetLatestByConversationAsync(Guid conversationId, CancellationToken ct);
    Task AddAsync(Message message, CancellationToken ct);
    Task AddRangeAsync(IEnumerable<Message> messages, CancellationToken ct);
    Task DeleteAsync(Message message, CancellationToken ct);
    Task DeleteDescendantsAsync(Guid messageId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}