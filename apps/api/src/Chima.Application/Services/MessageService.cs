using Chima.Application.Common;
using Chima.Application.DTOs.Message;
using Chima.Application.DTOs.AI;
using Chima.Application.Interfaces;
using Chima.Domain.Entities;
using Chima.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Chima.Application.Services;

public class MessageService
{
    private readonly IConversationRepository _conversations;
    private readonly IMessageRepository _messages;
    private readonly IAIService _ai;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<MessageService> _logger;

    public MessageService(
        IConversationRepository conversations,
        IMessageRepository messages,
        IAIService ai,
        IUnitOfWork uow,
        ILogger<MessageService> logger)
    {
        _conversations = conversations;
        _messages = messages;
        _ai = ai;
        _uow = uow;
        _logger = logger;
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
    /// Saves user message, calls AI, saves assistant message with real response.
    /// </summary>
    public async Task<(MessageDto UserMessage, MessageDto AssistantMessage)> SendAsync(
        Guid userId, Guid conversationId, SendMessageRequest request, CancellationToken ct)
    {
        var conversation = await EnsureOwnedAsync(conversationId, userId, ct);

        // 1. Persist user message
        var userMessage = new Message
        {
            ConversationId = conversationId,
            Role = MessageRole.User,
            Status = MessageStatus.Completed,
            Content = request.Content.Trim()
        };
        await _messages.AddAsync(userMessage, ct);
        await _uow.SaveChangesAsync(ct);

        // 2. Persist assistant placeholder (Pending)
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
        await _uow.SaveChangesAsync(ct);

        // 3. Auto-title
        if (conversation.Title == "New conversation")
        {
            try
            {
                var title = await _ai.GenerateTitleAsync(userMessage.Content, ct);
                if (!string.IsNullOrWhiteSpace(title)) conversation.Title = title;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Auto-title failed, keeping fallback title.");
            }
        }
        conversation.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);

        // 4. Fetch history for the AI
        var (history, _, _) = await _messages.ListByConversationAsync(
            conversationId, cursor: null, limit: 50, ascending: true, ct);

        // 5. Call AI
        try
        {
            var result = await _ai.CompleteForConversationAsync(
                conversation, history, userId, request.ModelId, ct);

            assistant.Content = result.Content;
            assistant.Status = MessageStatus.Completed;
            assistant.ModelId = result.ModelId;
            assistant.InputTokens = result.InputTokens;
            assistant.OutputTokens = result.OutputTokens;
            assistant.EstimatedCost = result.EstimatedCost;
            assistant.DurationMs = result.DurationMs;
            await _uow.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            assistant.Status = MessageStatus.Failed;
            assistant.ErrorCode = "ai_error";
            assistant.ErrorMessage = ex is AppException ae ? ae.Message : "AI request failed.";
            await _uow.SaveChangesAsync(ct);
            _logger.LogError(ex, "AI call failed for message {MessageId}.", assistant.Id);
        }

        return (ToDto(userMessage), ToDto(assistant));
    }

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

        await FillAssistantAsync(original.ConversationId, edited, assistant, userId, null, ct);

        return ToDto(edited);
    }

    public async Task DeleteAsync(Guid userId, Guid messageId, CancellationToken ct)
    {
        var msg = await _messages.GetOwnedAsync(messageId, userId, ct)
                  ?? throw new NotFoundException("Message not found.");

        await _messages.DeleteDescendantsAsync(msg.Id, ct);
        await _messages.DeleteAsync(msg, ct);
        await _uow.SaveChangesAsync(ct);
    }

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

        var userMessageId = original.ParentMessageId;
        if (userMessageId is { } uid)
        {
            var userMsg = await _messages.GetOwnedAsync(uid, userId, ct);
            if (userMsg is not null)
                await FillAssistantAsync(original.ConversationId, userMsg, sibling, userId, request.ModelId, ct);
        }

        return ToDto(sibling);
    }

    private async Task FillAssistantAsync(
        Guid conversationId, Message userMessage, Message assistant,
        Guid userId, Guid? preferredModelId, CancellationToken ct)
    {
        var conversation = await _conversations.GetOwnedAsync(conversationId, userId, ct);
        if (conversation is null) return;

        var (history, _, _) = await _messages.ListByConversationAsync(
            conversationId, cursor: null, limit: 50, ascending: true, ct);

        try
        {
            var result = await _ai.CompleteForConversationAsync(
                conversation, history, userId, preferredModelId, ct);

            assistant.Content = result.Content;
            assistant.Status = MessageStatus.Completed;
            assistant.ModelId = result.ModelId;
            assistant.InputTokens = result.InputTokens;
            assistant.OutputTokens = result.OutputTokens;
            assistant.EstimatedCost = result.EstimatedCost;
            assistant.DurationMs = result.DurationMs;
            await _uow.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            assistant.Status = MessageStatus.Failed;
            assistant.ErrorCode = "ai_error";
            assistant.ErrorMessage = ex is AppException ae ? ae.Message : "AI request failed.";
            await _uow.SaveChangesAsync(ct);
            _logger.LogError(ex, "AI call failed for message {MessageId}.", assistant.Id);
        }
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

    /// <summary>
    /// Streaming version of SendAsync. Writes user message + pending assistant,
    /// yields deltas to the caller, then finalizes the assistant message.
    /// </summary>
    public async IAsyncEnumerable<StreamEvent> StreamAsync(
        Guid userId, Guid conversationId, SendMessageRequest request,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        var conversation = await EnsureOwnedAsync(conversationId, userId, ct);

        // 1. Persist user message
        var userMessage = new Message
        {
            ConversationId = conversationId,
            Role = MessageRole.User,
            Status = MessageStatus.Completed,
            Content = request.Content.Trim()
        };
        await _messages.AddAsync(userMessage, ct);
        await _uow.SaveChangesAsync(ct);

        // 2. Persist pending assistant message
        var assistant = new Message
        {
            ConversationId = conversationId,
            Role = MessageRole.Assistant,
            Status = MessageStatus.Streaming,
            Content = string.Empty,
            ParentMessageId = userMessage.Id,
            ModelId = request.ModelId
        };
        await _messages.AddAsync(assistant, ct);
        await _uow.SaveChangesAsync(ct);

        // 3. Auto-title
        if (conversation.Title == "New conversation")
        {
            try
            {
                var title = await _ai.GenerateTitleAsync(userMessage.Content, ct);
                if (!string.IsNullOrWhiteSpace(title)) conversation.Title = title;
            }
            catch (Exception ex) { _logger.LogWarning(ex, "Auto-title failed."); }
            conversation.UpdatedAt = DateTime.UtcNow;
            await _uow.SaveChangesAsync(ct);
        }

        // 4. Fetch history
        var (history, _, _) = await _messages.ListByConversationAsync(
            conversationId, cursor: null, limit: 50, ascending: true, ct);

        // 5. Emit meta event with IDs so client can track
        yield return new StreamEvent
        {
            Event = "meta",
            Data = new
            {
                userMessageId = userMessage.Id,
                assistantMessageId = assistant.Id,
                conversationId = conversationId
            }
        };

        // 6. Stream and accumulate
        var ctx = new AIStreamContext();
        var buffer = new System.Text.StringBuilder();
        var sw = System.Diagnostics.Stopwatch.StartNew();
        int inTok = 0, outTok = 0;
        string? finishReason = null;
        bool completed = false;
        bool cancelled = false;
        string? errorCode = null;
        string? errorMessage = null;

        var enumerator = _ai.StreamForConversationAsync(
            conversation, history, userId, request.ModelId, ctx, ct).GetAsyncEnumerator(ct);

        try
        {
            while (true)
            {
                AIStreamChunk current;
                try
                {
                    if (!await enumerator.MoveNextAsync()) break;
                    current = enumerator.Current;
                }
                catch (OperationCanceledException) when (ct.IsCancellationRequested)
                {
                    cancelled = true; break;
                }
                catch (Exception ex)
                {
                    errorCode = "ai_error";
                    errorMessage = ex is AppException ae ? ae.Message : "AI request failed.";
                    _logger.LogError(ex, "Streaming failed for assistant message {Id}.", assistant.Id);
                    break;
                }

                if (current.InputTokens.HasValue) inTok = current.InputTokens.Value;
                if (current.OutputTokens.HasValue) outTok = current.OutputTokens.Value;
                if (current.FinishReason is not null) finishReason = current.FinishReason;

                if (!string.IsNullOrEmpty(current.Content))
                {
                    buffer.Append(current.Content);
                    yield return new StreamEvent
                    {
                        Event = "delta",
                        Data = new { content = current.Content }
                    };
                }

                if (current.IsFinal) { completed = true; break; }
            }
        }
        finally
        {
            await enumerator.DisposeAsync();
            sw.Stop();

            assistant.Content = buffer.ToString();
            assistant.ModelId = ctx.ModelId != Guid.Empty ? ctx.ModelId : assistant.ModelId;
            assistant.InputTokens = inTok;
            assistant.OutputTokens = outTok;
            assistant.EstimatedCost = ComputeCost(ctx, inTok, outTok);
            assistant.DurationMs = (int)sw.ElapsedMilliseconds;

            if (cancelled)
            {
                assistant.Status = MessageStatus.Cancelled;
            }
            else if (errorCode is not null)
            {
                assistant.Status = MessageStatus.Failed;
                assistant.ErrorCode = errorCode;
                assistant.ErrorMessage = errorMessage;
            }
            else
            {
                assistant.Status = MessageStatus.Completed;
            }

            try { await _uow.SaveChangesAsync(CancellationToken.None); }
            catch (Exception ex) { _logger.LogError(ex, "Failed to finalize assistant message {Id}.", assistant.Id); }
        }

        // 7. Terminal event
        if (cancelled)
        {
            yield return new StreamEvent { Event = "error", Data = new { code = "cancelled", message = "Cancelled by client." } };
        }
        else if (errorCode is not null)
        {
            yield return new StreamEvent { Event = "error", Data = new { code = errorCode, message = errorMessage ?? "AI request failed." } };
        }
        else if (completed)
        {
            yield return new StreamEvent
            {
                Event = "done",
                Data = new
                {
                    inputTokens = inTok,
                    outputTokens = outTok,
                    estimatedCost = ComputeCost(ctx, inTok, outTok),
                    durationMs = (int)sw.ElapsedMilliseconds,
                    finishReason = finishReason ?? "stop"
                }
            };
        }
    }

    private static decimal ComputeCost(AIStreamContext ctx, int inTok, int outTok)
    {
        var inCost = (decimal)inTok / 1_000_000m * ctx.InputPricePer1M;
        var outCost = (decimal)outTok / 1_000_000m * ctx.OutputPricePer1M;
        return Math.Round(inCost + outCost, 6);
    }

    public class StreamEvent
    {
        public string Event { get; set; } = string.Empty;
        public object Data { get; set; } = new();
    }
}
