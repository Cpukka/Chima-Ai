using Chima.Domain.Entities;

namespace Chima.Application.Interfaces;

public interface IConversationRepository
{
    Task<Conversation?> GetOwnedAsync(Guid id, Guid userId, CancellationToken ct, bool includeMessages = false);
    Task<(List<Conversation> Items, string? NextCursor, bool HasMore)> ListAsync(
        Guid userId, string? cursor, int limit, bool includeArchived, CancellationToken ct);
    Task<(List<Conversation> Items, string? NextCursor, bool HasMore)> SearchAsync(
        Guid userId, string query, string? cursor, int limit, CancellationToken ct);
    Task AddAsync(Conversation conversation, CancellationToken ct);
    Task<int> CountByUserAsync(Guid userId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}