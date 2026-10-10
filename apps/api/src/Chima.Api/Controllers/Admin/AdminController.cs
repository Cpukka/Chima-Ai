using Chima.Application.Common;
using Chima.Application.DTOs.Admin;
using Chima.Application.Interfaces;
using Chima.Application.Interfaces.Admin;
using Chima.Api.Filters;
using Chima.Application.Services.Admin;
using Microsoft.AspNetCore.Mvc;

namespace Chima.Api.Controllers.Admin;

[ApiController]
[Route("api/admin")]
[AdminOnly]
public class AdminController : ControllerBase
{
    private readonly IAdminMetricsService _metrics;
    private readonly IAdminListService _lists;
    private readonly IAdminUserService _users;
    private readonly IAuditLogService _audit;
    private readonly IAuditLogRepository _auditRepo;
    private readonly ICurrentUser _current;

    public AdminController(
        IAdminMetricsService metrics,
        IAdminListService lists,
        IAdminUserService users,
        IAuditLogService audit,
        IAuditLogRepository auditRepo,
        ICurrentUser current)
    {
        _metrics = metrics;
        _lists = lists;
        _users = users;
        _audit = audit;
        _auditRepo = auditRepo;
        _current = current;
    }

    // ---------- Dashboard ----------

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard([FromQuery] int days = 30, CancellationToken ct = default)
    {
        var m = await _metrics.GetDashboardAsync(days, ct);
        return Ok(ApiResponse<DashboardMetricsDto>.Ok(m));
    }

    [HttpGet("revenue")]
    public async Task<IActionResult> Revenue([FromQuery] int days = 30, CancellationToken ct = default)
    {
        var r = await _metrics.GetRevenueAsync(days, ct);
        return Ok(ApiResponse<RevenueMetricsDto>.Ok(r));
    }

    [HttpGet("ai-costs")]
    public async Task<IActionResult> AiCosts([FromQuery] int days = 30, CancellationToken ct = default)
    {
        var r = await _metrics.GetAiCostsAsync(days, ct);
        return Ok(ApiResponse<AiCostMetricsDto>.Ok(r));
    }

    // ---------- Users ----------

    [HttpGet("users")]
    public async Task<IActionResult> Users(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] string? plan = null,
        CancellationToken ct = default)
    {
        var result = await _users.ListAsync(page, pageSize, search, status, plan, ct);
        return Ok(ApiResponse<PagedAdminResult<AdminUserListItemDto>>.Ok(result));
    }

    [HttpGet("users/{id:guid}")]
    public async Task<IActionResult> User(Guid id, CancellationToken ct)
    {
        var dto = await _users.GetAsync(id, ct);
        return Ok(ApiResponse<AdminUserDetailDto>.Ok(dto));
    }

    [HttpPost("users/{id:guid}/suspend")]
    public async Task<IActionResult> Suspend(Guid id, [FromBody] SuspendUserRequest req, CancellationToken ct)
    {
        await _users.SuspendAsync(id, req.Reason, ct);
        await _audit.LogAsync(_current.UserId, _current.Email, "user.suspend", "User", id.ToString(),
            new { reason = req.Reason }, _current.IpAddress, Request.Headers.UserAgent.ToString(), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "User suspended."));
    }

    [HttpPost("users/{id:guid}/unsuspend")]
    public async Task<IActionResult> Unsuspend(Guid id, CancellationToken ct)
    {
        await _users.UnsuspendAsync(id, ct);
        await _audit.LogAsync(_current.UserId, _current.Email, "user.unsuspend", "User", id.ToString(),
            null, _current.IpAddress, Request.Headers.UserAgent.ToString(), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "User unsuspended."));
    }

    [HttpPost("users/{id:guid}/change-plan")]
    public async Task<IActionResult> ChangePlan(Guid id, [FromBody] AdminChangePlanRequest req, CancellationToken ct)
    {
        await _users.ChangePlanAsync(id, req.PlanCode, req.Reason, ct);
        await _audit.LogAsync(_current.UserId, _current.Email, "user.change_plan", "User", id.ToString(),
            new { planCode = req.PlanCode, reason = req.Reason }, _current.IpAddress, Request.Headers.UserAgent.ToString(), ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Plan changed."));
    }

    // ---------- Audit ----------

    [HttpGet("audit-logs")]
    public async Task<IActionResult> AuditLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? action = null,
        [FromQuery] Guid? actorId = null,
        CancellationToken ct = default)
    {
        var (items, total) = await _auditRepo.ListAsync(page, pageSize, action, actorId, ct);
        return Ok(ApiResponse<object>.Ok(new { items, total, page, pageSize }));
    }

    // ---------- Payments ----------

    [HttpGet("payments")]
    public async Task<IActionResult> Payments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] string? provider = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        CancellationToken ct = default)
    {
        var result = await _lists.ListPaymentsAsync(new Chima.Application.DTOs.Admin.AdminListQuery
        {
            Page = page,
            PageSize = pageSize,
            Search = search,
            Status = status,
            Provider = provider,
            From = from,
            To = to
        }, ct);
        return Ok(ApiResponse<PagedAdminResult<Chima.Application.DTOs.Admin.AdminPaymentListItemDto>>.Ok(result));
    }

    // ---------- Subscriptions ----------

    [HttpGet("subscriptions")]
    public async Task<IActionResult> Subscriptions(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 25,
        [FromQuery] string? search = null,
        [FromQuery] string? status = null,
        [FromQuery] string? planCode = null,
        CancellationToken ct = default)
    {
        var result = await _lists.ListSubscriptionsAsync(new Chima.Application.DTOs.Admin.AdminListQuery
        {
            Page = page,
            PageSize = pageSize,
            Search = search,
            Status = status,
            PlanCode = planCode
        }, ct);
        return Ok(ApiResponse<PagedAdminResult<Chima.Application.DTOs.Admin.AdminSubscriptionListItemDto>>.Ok(result));
    }

    // ---------- Plans dropdown ----------

    [HttpGet("plans")]
    public async Task<IActionResult> Plans(CancellationToken ct)
    {
        var plans = await _lists.ListPlansAsync(ct);
        return Ok(ApiResponse<IReadOnlyList<Chima.Application.DTOs.Admin.AdminPlanOptionDto>>.Ok(plans));
    }
}
