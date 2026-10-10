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
    [Chima.Application.Common.RequireQuota("ai_message")]
    public async Task<IActionResult> SendMessage(
        Guid id,
        [FromBody] SendMessageRequest request,
        CancellationToken ct)
    {
        var (userMessage, assistantPlaceholder) = await _messages.SendAsync(UserId, id, request, ct);
        return Ok(ApiResponse<object>.Ok(new { userMessage, assistantPlaceholder }, "Message sent."));
    }

    [HttpPost("{id:guid}/messages/stream")]
    [Chima.Application.Common.RequireQuota("ai_message")]
    public async Task StreamMessage(Guid id, [FromBody] SendMessageRequest request, CancellationToken ct)
    {
        Chima.Application.Services.SseWriter.PrepareHeaders(Response);

        try
        {
            await foreach (var evt in _messages.StreamAsync(UserId, id, request, ct))
            {
                await Chima.Application.Services.SseWriter.WriteEventAsync(Response, evt.Event, evt.Data, ct);
            }
        }
        catch (OperationCanceledException)
        {
            // Client disconnected â€” nothing else to do; finalization happened in finally block.
        }
        catch (Exception ex)
        {
            try
            {
                await Chima.Application.Services.SseWriter.WriteEventAsync(
                    Response, "error", new { code = "server_error", message = ex.Message }, CancellationToken.None);
            }
            catch { /* response already gone */ }
        }
    }
}
