using Chima.Application.Common;
using Chima.Application.DTOs.Admin;
using Chima.Application.Interfaces;
using Chima.Application.Interfaces.Admin;
using Chima.Api.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Chima.Api.Controllers.Admin;

[ApiController]
[Route("api/admin")]
[AdminOnly]
public class AdminSettingsController : ControllerBase
{
    private readonly IAdminSettingsService _settings;
    private readonly IAuditLogService _audit;
    private readonly ICurrentUser _current;

    public AdminSettingsController(IAdminSettingsService settings, IAuditLogService audit, ICurrentUser current)
    {
        _settings = settings; _audit = audit; _current = current;
    }

    [HttpGet("feature-flags")]
    public async Task<IActionResult> ListFlags(CancellationToken ct)
    {
        var list = await _settings.ListFeatureFlagsAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<FeatureFlagDto>>.Ok(list));
    }

    [HttpPatch("feature-flags/{key}")]
    public async Task<IActionResult> UpdateFlag(string key, [FromBody] UpdateFeatureFlagRequest req, CancellationToken ct)
    {
        var flag = await _settings.UpdateFeatureFlagAsync(key, req, ct);
        await _audit.LogAsync(_current.UserId, _current.Email, "settings.update_flag", "FeatureFlag", key,
            new { isEnabled = flag.IsEnabled, rollout = flag.RolloutPercent },
            _current.IpAddress, Request.Headers.UserAgent.ToString(), ct);
        return Ok(ApiResponse<FeatureFlagDto>.Ok(flag, "Feature flag updated."));
    }

    [HttpGet("system-settings")]
    public async Task<IActionResult> ListSettings(CancellationToken ct)
    {
        var list = await _settings.ListSystemSettingsAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<SystemSettingDto>>.Ok(list));
    }

    [HttpPatch("system-settings/{key}")]
    public async Task<IActionResult> UpdateSetting(string key, [FromBody] UpdateSystemSettingRequest req, CancellationToken ct)
    {
        var s = await _settings.UpdateSystemSettingAsync(key, req, ct);
        await _audit.LogAsync(_current.UserId, _current.Email, "settings.update_setting", "SystemSetting", key,
            null, _current.IpAddress, Request.Headers.UserAgent.ToString(), ct);
        return Ok(ApiResponse<SystemSettingDto>.Ok(s, "Setting updated."));
    }
}