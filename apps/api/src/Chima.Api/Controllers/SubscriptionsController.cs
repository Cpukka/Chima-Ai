using Chima.Application.Common;
using Chima.Application.DTOs.Subscription;
using Chima.Application.Interfaces;
using Chima.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chima.Api.Controllers;

[ApiController]
[Route("api/subscriptions")]
public class SubscriptionsController : ControllerBase
{
    private readonly ISubscriptionService _subscriptions;
    private readonly ICurrentUser _current;

    public SubscriptionsController(ISubscriptionService subscriptions, ICurrentUser current)
    {
        _subscriptions = subscriptions;
        _current = current;
    }

    /// <summary>Public plan list — no auth required.</summary>
    [HttpGet("plans")]
    [AllowAnonymous]
    public async Task<IActionResult> Plans(CancellationToken ct)
    {
        var plans = await _subscriptions.GetPlansAsync(includeInactive: false, ct);
        return Ok(ApiResponse<IReadOnlyList<PlanDto>>.Ok(plans));
    }

    /// <summary>Current user's plan, usage, and limits.</summary>
    [HttpGet("current")]
    [Authorize]
    public async Task<IActionResult> Current(CancellationToken ct)
    {
        var userId = _current.UserId ?? throw new UnauthorizedException();
        var status = await _subscriptions.GetCurrentAsync(userId, ct);
        return Ok(ApiResponse<SubscriptionStatusDto>.Ok(status));
    }

    /// <summary>Cancel auto-renewal (subscription stays active until period end).</summary>
    [HttpPost("cancel")]
    [Authorize]
    public async Task<IActionResult> Cancel(CancellationToken ct)
    {
        var userId = _current.UserId ?? throw new UnauthorizedException();
        var status = await _subscriptions.CancelAsync(userId, ct);
        return Ok(ApiResponse<SubscriptionStatusDto>.Ok(status, "Subscription cancelled."));
    }

    /// <summary>Undo cancellation.</summary>
    [HttpPost("restore")]
    [Authorize]
    public async Task<IActionResult> Restore(CancellationToken ct)
    {
        var userId = _current.UserId ?? throw new UnauthorizedException();
        var status = await _subscriptions.RestoreAsync(userId, ct);
        return Ok(ApiResponse<SubscriptionStatusDto>.Ok(status, "Subscription restored."));
    }

    /// <summary>
    /// Development/testing only: change plan without going through Stripe or Google Play.
    /// Disabled automatically in production (see body).
    /// </summary>
    [HttpPost("dev/change-plan")]
    [Authorize]
    public async Task<IActionResult> DevChangePlan(
        [FromBody] ChangePlanRequest request,
        [FromServices] IWebHostEnvironment env,
        CancellationToken ct)
    {
        if (env.IsProduction())
            return NotFound();

        var userId = _current.UserId ?? throw new UnauthorizedException();
        var status = await _subscriptions.ChangePlanAsync(
            userId,
            request.PlanCode,
            request.Provider ?? "manual",
            request.ExternalId,
            request.PeriodEnd,
            ct);

        return Ok(ApiResponse<SubscriptionStatusDto>.Ok(status, $"Switched to {status.PlanName}."));
    }
}