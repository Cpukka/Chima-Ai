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