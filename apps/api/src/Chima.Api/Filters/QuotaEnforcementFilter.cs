using Chima.Application.Common;
using Chima.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Chima.Api.Filters;

/// <summary>
/// Blocks the request with 402 Payment Required if the authenticated user
/// has exceeded their monthly quota for the feature marked on the endpoint.
/// </summary>
public class QuotaEnforcementFilter : IAsyncActionFilter
{
    private readonly ISubscriptionService _subscriptions;
    private readonly ICurrentUser _current;

    public QuotaEnforcementFilter(ISubscriptionService subscriptions, ICurrentUser current)
    {
        _subscriptions = subscriptions;
        _current = current;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var attr = context.ActionDescriptor.EndpointMetadata.OfType<RequireQuotaAttribute>().FirstOrDefault();
        if (attr is null)
        {
            await next();
            return;
        }

        var userId = _current.UserId;
        if (userId is null)
        {
            await next();
            return;
        }

        var check = await _subscriptions.CheckQuotaAsync(userId.Value, attr.Feature, context.HttpContext.RequestAborted);
        if (!check.Allowed)
        {
            context.Result = new ObjectResult(new
            {
                success = false,
                data = (object?)null,
                message = check.BlockReason,
                errors = new[]
                {
                    new { code = "quota_exceeded", message = check.BlockReason, field = (string?)null }
                },
                meta = new
                {
                    feature = check.Feature,
                    used = check.Used,
                    limit = check.Limit,
                    remaining = check.Remaining,
                    resetAt = check.ResetAt
                }
            })
            {
                StatusCode = 402
            };
            return;
        }

        await next();
    }
}