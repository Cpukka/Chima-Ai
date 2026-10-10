using Chima.Application.Common;
using Chima.Application.DTOs.Admin;
using Chima.Application.Interfaces;
using Chima.Application.Interfaces.Admin;
using Chima.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Chima.Api.Controllers.Admin;

[ApiController]
[Route("api/admin/ai")]
[AdminOnly]
public class AdminAiController : ControllerBase
{
    private readonly IAdminAiService _ai;
    private readonly IAuditLogService _audit;
    private readonly ICurrentUser _current;

    public AdminAiController(IAdminAiService ai, IAuditLogService audit, ICurrentUser current)
    {
        _ai = ai; _audit = audit; _current = current;
    }

    [HttpGet("providers")]
    public async Task<IActionResult> ListProviders(CancellationToken ct)
    {
        var p = await _ai.ListProvidersAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<AdminAiProviderDto>>.Ok(p));
    }

    [HttpPost("providers")]
    public async Task<IActionResult> CreateProvider([FromBody] CreateAiProviderRequest req, CancellationToken ct)
    {
        var p = await _ai.CreateProviderAsync(req, ct);
        await _audit.LogAsync(_current.UserId, _current.Email, "ai.create_provider", "AIProvider", p.Id.ToString(),
            new { name = p.Name }, _current.IpAddress, Request.Headers.UserAgent.ToString(), ct);
        return Ok(ApiResponse<AdminAiProviderDto>.Ok(p, "Provider created."));
    }

    [HttpPatch("providers/{id:guid}")]
    public async Task<IActionResult> UpdateProvider(Guid id, [FromBody] UpdateAiProviderRequest req, CancellationToken ct)
    {
        var p = await _ai.UpdateProviderAsync(id, req, ct);
        await _audit.LogAsync(_current.UserId, _current.Email, "ai.update_provider", "AIProvider", id.ToString(),
            null, _current.IpAddress, Request.Headers.UserAgent.ToString(), ct);
        return Ok(ApiResponse<AdminAiProviderDto>.Ok(p, "Provider updated."));
    }

    [HttpGet("models")]
    public async Task<IActionResult> ListModels(CancellationToken ct)
    {
        var m = await _ai.ListModelsAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<AdminAiModelDto>>.Ok(m));
    }

    [HttpPost("models")]
    public async Task<IActionResult> CreateModel([FromBody] CreateAiModelRequest req, CancellationToken ct)
    {
        var m = await _ai.CreateModelAsync(req, ct);
        await _audit.LogAsync(_current.UserId, _current.Email, "ai.create_model", "AIModel", m.Id.ToString(),
            new { name = m.Name }, _current.IpAddress, Request.Headers.UserAgent.ToString(), ct);
        return Ok(ApiResponse<AdminAiModelDto>.Ok(m, "Model created."));
    }

    [HttpPatch("models/{id:guid}")]
    public async Task<IActionResult> UpdateModel(Guid id, [FromBody] UpdateAiModelRequest req, CancellationToken ct)
    {
        var m = await _ai.UpdateModelAsync(id, req, ct);
        await _audit.LogAsync(_current.UserId, _current.Email, "ai.update_model", "AIModel", id.ToString(),
            null, _current.IpAddress, Request.Headers.UserAgent.ToString(), ct);
        return Ok(ApiResponse<AdminAiModelDto>.Ok(m, "Model updated."));
    }

    [HttpPost("models/{id:guid}/set-default")]
    public async Task<IActionResult> SetDefault(Guid id, CancellationToken ct)
    {
        await _ai.SetDefaultModelAsync(id, ct);
        await _audit.LogAsync(_current.UserId, _current.Email, "ai.set_default_model", "AIModel", id.ToString(),
            null, _current.IpAddress, Request.Headers.UserAgent.ToString(), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Default model updated."));
    }
}