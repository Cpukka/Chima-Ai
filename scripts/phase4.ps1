# =====================================================================
#  CHIMA — PHASE 4 SETUP SCRIPT
#  Conversations & Messages (storage + API, no AI yet)
# =====================================================================

$ErrorActionPreference = "Stop"
$root = "C:\Users\Chima\chima\apps\api"
if (-not (Test-Path $root)) { Write-Host "ERROR: $root missing." -ForegroundColor Red; exit 1 }
Set-Location $root

function Write-File($path, $content) {
    $full = Join-Path $root $path
    $dir = Split-Path $full -Parent
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
    [System.IO.File]::WriteAllText($full, $content)
    Write-Host "  wrote $path" -ForegroundColor DarkGray
}

Write-Host "=== Phase 4: folders ===" -ForegroundColor Cyan
$dirs = @(
  "src\Chima.Application\DTOs\Conversation",
  "src\Chima.Application\DTOs\Message",
  "src\Chima.Application\Validators\Conversation",
  "src\Chima.Infrastructure\Persistence\Repositories"
)
foreach ($d in $dirs) { New-Item -ItemType Directory -Force -Path (Join-Path $root $d) | Out-Null }

# ============ APPLICATION — common ============

Write-Host "=== Phase 4: Application common ===" -ForegroundColor Cyan

Write-File "src\Chima.Application\Common\PagedResult.cs" @'
namespace Chima.Application.Common;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public string? NextCursor { get; init; }
    public bool HasMore { get; init; }
    public int? Total { get; init; }
}
'@

# ============ APPLICATION — repository interfaces ============

Write-File "src\Chima.Application\Interfaces\IConversationRepository.cs" @'
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
'@

Write-File "src\Chima.Application\Interfaces\IMessageRepository.cs" @'
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
'@

# ============ APPLICATION — DTOs ============

Write-Host "=== Phase 4: DTOs ===" -ForegroundColor Cyan

Write-File "src\Chima.Application\DTOs\Conversation\ConversationDtos.cs" @'
namespace Chima.Application.DTOs.Conversation;

public class ConversationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int MessageCount { get; set; }
    public string? LastMessagePreview { get; set; }
    public DateTime? LastMessageAt { get; set; }
}

public class CreateConversationRequest
{
    public string? Title { get; set; }
}

public class UpdateConversationRequest
{
    public string? Title { get; set; }
    public bool? IsPinned { get; set; }
    public bool? IsArchived { get; set; }
}
'@

Write-File "src\Chima.Application\DTOs\Message\MessageDtos.cs" @'
namespace Chima.Application.DTOs.Message;

public class MessageDto
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public string Role { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid? ParentMessageId { get; set; }
    public Guid? BranchRootId { get; set; }
    public Guid? ModelId { get; set; }
    public int? InputTokens { get; set; }
    public int? OutputTokens { get; set; }
    public decimal? EstimatedCost { get; set; }
    public int? DurationMs { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SendMessageRequest
{
    public string Content { get; set; } = string.Empty;
    public Guid? ModelId { get; set; }
}

public class EditMessageRequest
{
    public string Content { get; set; } = string.Empty;
}

public class RegenerateRequest
{
    public Guid? ModelId { get; set; }
}
'@

# ============ APPLICATION — validators ============

Write-Host "=== Phase 4: validators ===" -ForegroundColor Cyan

Write-File "src\Chima.Application\Validators\Conversation\ConversationValidators.cs" @'
using Chima.Application.DTOs.Conversation;
using Chima.Application.DTOs.Message;
using FluentValidation;

namespace Chima.Application.Validators.Conversation;

public class CreateConversationRequestValidator : AbstractValidator<CreateConversationRequest>
{
    public CreateConversationRequestValidator()
    {
        RuleFor(x => x.Title).MaximumLength(200);
    }
}

public class UpdateConversationRequestValidator : AbstractValidator<UpdateConversationRequest>
{
    public UpdateConversationRequestValidator()
    {
        RuleFor(x => x.Title).MaximumLength(200);
    }
}

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(32_000);
    }
}

public class EditMessageRequestValidator : AbstractValidator<EditMessageRequest>
{
    public EditMessageRequestValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(32_000);
    }
}
'@

# ============ APPLICATION — services ============

Write-Host "=== Phase 4: services ===" -ForegroundColor Cyan

Write-File "src\Chima.Application\Services\ConversationService.cs" @'
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
'@

Write-File "src\Chima.Application\Services\MessageService.cs" @'
using Chima.Application.Common;
using Chima.Application.DTOs.Message;
using Chima.Application.Interfaces;
using Chima.Domain.Entities;
using Chima.Domain.Enums;

namespace Chima.Application.Services;

public class MessageService
{
    private readonly IConversationRepository _conversations;
    private readonly IMessageRepository _messages;
    private readonly IUnitOfWork _uow;

    public MessageService(
        IConversationRepository conversations,
        IMessageRepository messages,
        IUnitOfWork uow)
    {
        _conversations = conversations;
        _messages = messages;
        _uow = uow;
    }

    public async Task<PagedResult<MessageDto>> ListAsync(
        Guid userId, Guid conversationId, string? cursor, int limit, bool ascending, CancellationToken ct)
    {
        await EnsureOwnedAsync(conversationId, userId, ct);
        limit = Math.Clamp(limit, 1, 100);
        var (items, next, more) = await _messages.ListByConversationAsync(conversationId, cursor, limit, ascending, ct);

        return new PagedResult<MessageDto>
        {
            Items = items.Select(ToDto).ToList(),
            NextCursor = next,
            HasMore = more
        };
    }

    /// <summary>
    /// Persists a user message and creates a placeholder assistant message (Pending).
    /// Phase 5 fills the assistant message with a real response.
    /// </summary>
    public async Task<(MessageDto UserMessage, MessageDto AssistantPlaceholder)> SendAsync(
        Guid userId, Guid conversationId, SendMessageRequest request, CancellationToken ct)
    {
        var conversation = await EnsureOwnedAsync(conversationId, userId, ct);

        var userMessage = new Message
        {
            ConversationId = conversationId,
            Role = MessageRole.User,
            Status = MessageStatus.Completed,
            Content = request.Content.Trim()
        };
        await _messages.AddAsync(userMessage, ct);
        await _uow.SaveChangesAsync(ct);

        var assistant = new Message
        {
            ConversationId = conversationId,
            Role = MessageRole.Assistant,
            Status = MessageStatus.Pending,
            Content = string.Empty,
            ParentMessageId = userMessage.Id,
            ModelId = request.ModelId
        };
        await _messages.AddAsync(assistant, ct);

        // Auto-title on first user message
        if (conversation.Title == "New conversation")
        {
            conversation.Title = AutoTitle(request.Content);
        }
        conversation.UpdatedAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync(ct);

        return (ToDto(userMessage), ToDto(assistant));
    }

    /// <summary>
    /// Edits a user message by creating a new branch — the original is preserved.
    /// </summary>
    public async Task<MessageDto> EditAsync(Guid userId, Guid messageId, EditMessageRequest request, CancellationToken ct)
    {
        var original = await _messages.GetOwnedAsync(messageId, userId, ct)
                       ?? throw new NotFoundException("Message not found.");

        if (original.Role != MessageRole.User)
            throw new ValidationException("Only user messages can be edited.");

        var branchRoot = original.BranchRootId ?? original.Id;

        var edited = new Message
        {
            ConversationId = original.ConversationId,
            Role = MessageRole.User,
            Status = MessageStatus.Completed,
            Content = request.Content.Trim(),
            ParentMessageId = original.ParentMessageId,
            BranchRootId = branchRoot
        };
        await _messages.AddAsync(edited, ct);

        var assistant = new Message
        {
            ConversationId = original.ConversationId,
            Role = MessageRole.Assistant,
            Status = MessageStatus.Pending,
            Content = string.Empty,
            ParentMessageId = edited.Id
        };
        await _messages.AddAsync(assistant, ct);
        await _uow.SaveChangesAsync(ct);

        return ToDto(edited);
    }

    public async Task DeleteAsync(Guid userId, Guid messageId, CancellationToken ct)
    {
        var msg = await _messages.GetOwnedAsync(messageId, userId, ct)
                  ?? throw new NotFoundException("Message not found.");

        // Delete descendants first (branch children)
        await _messages.DeleteDescendantsAsync(msg.Id, ct);
        await _messages.DeleteAsync(msg, ct);
        await _uow.SaveChangesAsync(ct);
    }

    /// <summary>
    /// Creates a sibling assistant message alongside a previous assistant answer.
    /// Phase 5 fills this with a real new response.
    /// </summary>
    public async Task<MessageDto> RegenerateAsync(Guid userId, Guid messageId, RegenerateRequest request, CancellationToken ct)
    {
        var original = await _messages.GetOwnedAsync(messageId, userId, ct)
                       ?? throw new NotFoundException("Message not found.");

        if (original.Role != MessageRole.Assistant)
            throw new ValidationException("Only assistant messages can be regenerated.");

        var branchRoot = original.BranchRootId ?? original.Id;

        var sibling = new Message
        {
            ConversationId = original.ConversationId,
            Role = MessageRole.Assistant,
            Status = MessageStatus.Pending,
            Content = string.Empty,
            ParentMessageId = original.ParentMessageId,
            BranchRootId = branchRoot,
            ModelId = request.ModelId ?? original.ModelId
        };
        await _messages.AddAsync(sibling, ct);
        await _uow.SaveChangesAsync(ct);

        return ToDto(sibling);
    }

    private async Task<Conversation> EnsureOwnedAsync(Guid conversationId, Guid userId, CancellationToken ct)
        => await _conversations.GetOwnedAsync(conversationId, userId, ct)
           ?? throw new NotFoundException("Conversation not found.");

    private static MessageDto ToDto(Message m) => new()
    {
        Id = m.Id,
        ConversationId = m.ConversationId,
        Role = m.Role.ToString().ToLowerInvariant(),
        Status = m.Status.ToString().ToLowerInvariant(),
        Content = m.Content,
        ParentMessageId = m.ParentMessageId,
        BranchRootId = m.BranchRootId,
        ModelId = m.ModelId,
        InputTokens = m.InputTokens,
        OutputTokens = m.OutputTokens,
        EstimatedCost = m.EstimatedCost,
        DurationMs = m.DurationMs,
        ErrorCode = m.ErrorCode,
        ErrorMessage = m.ErrorMessage,
        CreatedAt = m.CreatedAt,
        UpdatedAt = m.UpdatedAt
    };

    private static string AutoTitle(string firstMessage)
    {
        var text = firstMessage.Trim();
        if (text.Length <= 60) return text;
        var cut = text.LastIndexOf(' ', 60);
        return (cut > 30 ? text[..cut] : text[..60]) + "…";
    }
}
'@

# ============ INFRASTRUCTURE — repositories ============

Write-Host "=== Phase 4: repositories ===" -ForegroundColor Cyan

Write-File "src\Chima.Infrastructure\Persistence\Repositories\ConversationRepository.cs" @'
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
'@

Write-File "src\Chima.Infrastructure\Persistence\Repositories\MessageRepository.cs" @'
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
'@

# ============ INFRASTRUCTURE — register repositories ============

Write-File "src\Chima.Infrastructure\Configuration\InfrastructureServiceExtensions.cs" @'
using Chima.Application.Interfaces;
using Chima.Application.Settings;
using Chima.Infrastructure.Authentication;
using Chima.Infrastructure.Email;
using Chima.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chima.Infrastructure.Configuration;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddChimaAuthInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtSettings>(config.GetSection(JwtSettings.SectionName));
        services.Configure<GoogleSettings>(config.GetSection(GoogleSettings.SectionName));
        services.Configure<AppSettings>(config.GetSection(AppSettings.SectionName));

        services.AddHttpContextAccessor();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddSingleton<IEmailService, ConsoleEmailService>();
        services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();

        // Auth repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Conversations
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        return services;
    }
}
'@

# ============ API — controllers ============

Write-Host "=== Phase 4: controllers ===" -ForegroundColor Cyan

Write-File "src\Chima.Api\Controllers\ConversationsController.cs" @'
using Chima.Application.Common;
using Chima.Application.DTOs.Conversation;
using Chima.Application.DTOs.Message;
using Chima.Application.Interfaces;
using Chima.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chima.Api.Controllers;

[ApiController]
[Route("api/conversations")]
[Authorize]
public class ConversationsController : ControllerBase
{
    private readonly ConversationService _conversations;
    private readonly MessageService _messages;
    private readonly ICurrentUser _current;

    public ConversationsController(
        ConversationService conversations,
        MessageService messages,
        ICurrentUser current)
    {
        _conversations = conversations;
        _messages = messages;
        _current = current;
    }

    private Guid UserId => _current.UserId ?? throw new UnauthorizedException();

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? cursor,
        [FromQuery] int limit = 25,
        [FromQuery] bool includeArchived = false,
        CancellationToken ct = default)
    {
        var result = await _conversations.ListAsync(UserId, cursor, limit, includeArchived, ct);
        return Ok(ApiResponse<PagedResult<ConversationDto>>.Ok(result));
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] string? cursor,
        [FromQuery] int limit = 25,
        CancellationToken ct = default)
    {
        var result = await _conversations.SearchAsync(UserId, q, cursor, limit, ct);
        return Ok(ApiResponse<PagedResult<ConversationDto>>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateConversationRequest request, CancellationToken ct)
    {
        var result = await _conversations.CreateAsync(UserId, request, ct);
        return Ok(ApiResponse<ConversationDto>.Ok(result, "Conversation created."));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken ct)
    {
        var result = await _conversations.GetAsync(UserId, id, ct);
        return Ok(ApiResponse<ConversationDto>.Ok(result));
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateConversationRequest request, CancellationToken ct)
    {
        var result = await _conversations.UpdateAsync(UserId, id, request, ct);
        return Ok(ApiResponse<ConversationDto>.Ok(result, "Conversation updated."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _conversations.DeleteAsync(UserId, id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Conversation deleted."));
    }

    [HttpPost("{id:guid}/restore")]
    public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
    {
        await _conversations.RestoreAsync(UserId, id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Conversation restored."));
    }

    // ---------- Messages nested under conversation ----------

    [HttpGet("{id:guid}/messages")]
    public async Task<IActionResult> ListMessages(
        Guid id,
        [FromQuery] string? cursor,
        [FromQuery] int limit = 50,
        [FromQuery] bool ascending = true,
        CancellationToken ct = default)
    {
        var result = await _messages.ListAsync(UserId, id, cursor, limit, ascending, ct);
        return Ok(ApiResponse<PagedResult<MessageDto>>.Ok(result));
    }

    [HttpPost("{id:guid}/messages")]
    public async Task<IActionResult> SendMessage(
        Guid id,
        [FromBody] SendMessageRequest request,
        CancellationToken ct)
    {
        var (userMessage, assistantPlaceholder) = await _messages.SendAsync(UserId, id, request, ct);
        return Ok(ApiResponse<object>.Ok(new { userMessage, assistantPlaceholder }, "Message sent."));
    }
}
'@

Write-File "src\Chima.Api\Controllers\MessagesController.cs" @'
using Chima.Application.Common;
using Chima.Application.DTOs.Message;
using Chima.Application.Interfaces;
using Chima.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chima.Api.Controllers;

[ApiController]
[Route("api/messages")]
[Authorize]
public class MessagesController : ControllerBase
{
    private readonly MessageService _messages;
    private readonly ICurrentUser _current;

    public MessagesController(MessageService messages, ICurrentUser current)
    {
        _messages = messages;
        _current = current;
    }

    private Guid UserId => _current.UserId ?? throw new UnauthorizedException();

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] EditMessageRequest request, CancellationToken ct)
    {
        var result = await _messages.EditAsync(UserId, id, request, ct);
        return Ok(ApiResponse<MessageDto>.Ok(result, "Message edited."));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _messages.DeleteAsync(UserId, id, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Message deleted."));
    }

    [HttpPost("{id:guid}/regenerate")]
    public async Task<IActionResult> Regenerate(Guid id, [FromBody] RegenerateRequest request, CancellationToken ct)
    {
        var result = await _messages.RegenerateAsync(UserId, id, request, ct);
        return Ok(ApiResponse<MessageDto>.Ok(result, "Regeneration queued."));
    }
}
'@

# ============ register services in DI ============

Write-Host "=== Phase 4: DI registration ===" -ForegroundColor Cyan

Write-File "src\Chima.Api\Extensions\ServiceCollectionExtensions.cs" @'
using Chima.Application.Interfaces;
using Chima.Application.Services;
using Chima.Infrastructure.Configuration;
using Chima.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

namespace Chima.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChimaPersistence(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("Default")
                 ?? throw new InvalidOperationException("Connection string 'Default' missing.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(cs, npg =>
            {
                npg.UseVector();
                npg.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            }));

        return services;
    }

    public static IServiceCollection AddChimaAuth(this IServiceCollection services, IConfiguration config)
    {
        var jwt = config.GetSection("Jwt");
        var secret = jwt["Secret"] ?? throw new InvalidOperationException("Jwt:Secret missing.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization();
        services.AddChimaAuthInfrastructure(config);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ConversationService>();
        services.AddScoped<MessageService>();

        services.AddValidatorsFromAssembly(typeof(AuthService).Assembly);

        services.AddRateLimiter(options =>
        {
            options.AddPolicy("auth", context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.RejectionStatusCode = 429;
        });

        return services;
    }

    public static IServiceCollection AddChimaSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chima API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT access token."
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }

    public static IServiceCollection AddChimaCors(this IServiceCollection services, IConfiguration config)
    {
        var origins = (config["Cors:AllowedOrigins"] ?? "http://localhost:3000,http://localhost:3001")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        services.AddCors(o => o.AddPolicy("Default", p =>
            p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

        return services;
    }
}
'@

# ============ build ============

Write-Host "=== Phase 4: build ===" -ForegroundColor Cyan
dotnet restore | Out-Host
dotnet build --nologo | Out-Host

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed. Fix errors above." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "=== Phase 4 complete. ===" -ForegroundColor Green
Write-Host ""
Write-Host "Next:" -ForegroundColor Yellow
Write-Host "  1. Stop any running API (Ctrl+C in the other terminal)."
Write-Host "  2. Start the API:"
Write-Host "       dotnet run --project src\Chima.Api"
Write-Host "  3. Open http://localhost:5080/swagger"
Write-Host ""