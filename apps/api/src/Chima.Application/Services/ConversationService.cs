using Chima.Application.Common;
using Chima.Application.DTOs.Conversation;
using Chima.Application.Interfaces;
using Chima.Domain.Entities;

namespace Chima.Application.Services;

public class ConversationService
{
    private readonly IConversationRepository _conversations;
    private readonly IMessageRepository _messages;
    private readonly IUnitOfWork _uow;

    public ConversationService(
        IConversationRepository conversations,
        IMessageRepository messages,
        IUnitOfWork uow)
    {
        _conversations = conversations;
        _messages = messages;
        _uow = uow;
    }

    public async Task<PagedResult<ConversationDto>> ListAsync(
        Guid userId, string? cursor, int limit, bool includeArchived, CancellationToken ct)
    {
        limit = Math.Clamp(limit, 1, 100);
        var (items, next, more) = await _conversations.ListAsync(userId, cursor, limit, includeArchived, ct);

        var dtos = new List<ConversationDto>(items.Count);
        foreach (var c in items)
            dtos.Add(await ToDtoAsync(c, ct));

        return new PagedResult<ConversationDto>
        {
            Items = dtos,
            NextCursor = next,
            HasMore = more
        };
    }

    public async Task<PagedResult<ConversationDto>> SearchAsync(
        Guid userId, string query, string? cursor, int limit, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(query)) return new PagedResult<ConversationDto>();
        limit = Math.Clamp(limit, 1, 50);
        var (items, next, more) = await _conversations.SearchAsync(userId, query.Trim(), cursor, limit, ct);

        var dtos = new List<ConversationDto>(items.Count);
        foreach (var c in items)
            dtos.Add(await ToDtoAsync(c, ct));

        return new PagedResult<ConversationDto>
        {
            Items = dtos,
            NextCursor = next,
            HasMore = more
        };
    }

    public async Task<ConversationDto> CreateAsync(Guid userId, CreateConversationRequest request, CancellationToken ct)
    {
        var conversation = new Conversation
        {
            UserId = userId,
            Title = string.IsNullOrWhiteSpace(request.Title) ? "New conversation" : request.Title!.Trim()
        };
        await _conversations.AddAsync(conversation, ct);
        await _uow.SaveChangesAsync(ct);
        return await ToDtoAsync(conversation, ct);
    }

    public async Task<ConversationDto> GetAsync(Guid userId, Guid id, CancellationToken ct)
    {
        var c = await _conversations.GetOwnedAsync(id, userId, ct)
                ?? throw new NotFoundException("Conversation not found.");
        return await ToDtoAsync(c, ct);
    }

    public async Task<ConversationDto> UpdateAsync(Guid userId, Guid id, UpdateConversationRequest request, CancellationToken ct)
    {
        var c = await _conversations.GetOwnedAsync(id, userId, ct)
                ?? throw new NotFoundException("Conversation not found.");

        if (request.Title is not null) c.Title = request.Title.Trim();
        if (request.IsPinned.HasValue) c.IsPinned = request.IsPinned.Value;
        if (request.IsArchived.HasValue) c.IsArchived = request.IsArchived.Value;

        await _uow.SaveChangesAsync(ct);
        return await ToDtoAsync(c, ct);
    }

    public async Task DeleteAsync(Guid userId, Guid id, CancellationToken ct)
    {
        var c = await _conversations.GetOwnedAsync(id, userId, ct)
                ?? throw new NotFoundException("Conversation not found.");
        c.DeletedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task RestoreAsync(Guid userId, Guid id, CancellationToken ct)
    {
        var c = await _conversations.GetOwnedAsync(id, userId, ct)
                ?? throw new NotFoundException("Conversation not found.");
        c.DeletedAt = null;
        await _uow.SaveChangesAsync(ct);
    }

    private async Task<ConversationDto> ToDtoAsync(Conversation c, CancellationToken ct)
    {
        var last = await _messages.GetLatestByConversationAsync(c.Id, ct);
        var count = c.Messages?.Count ?? 0;

        return new ConversationDto
        {
            Id = c.Id,
            Title = c.Title,
            IsPinned = c.IsPinned,
            IsArchived = c.IsArchived,
            DeletedAt = c.DeletedAt,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt,
            MessageCount = count,
            LastMessagePreview = last is null ? null : Truncate(last.Content, 120),
            LastMessageAt = last?.CreatedAt
        };
    }

    private static string Truncate(string s, int max) =>
        string.IsNullOrEmpty(s) ? s : (s.Length <= max ? s : s[..max] + "…");
}