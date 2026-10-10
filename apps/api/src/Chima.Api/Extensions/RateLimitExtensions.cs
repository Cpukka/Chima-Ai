using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace Chima.Api.Extensions;

public static class RateLimitExtensions
{
    public static IServiceCollection AddChimaRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Auth endpoints — strict per-IP
            options.AddPolicy("auth", context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));

            // File uploads — per-user, moderate
            options.AddPolicy("uploads", context =>
            {
                var userId = context.User?.FindFirst("sub")?.Value
                             ?? context.Connection.RemoteIpAddress?.ToString()
                             ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(userId, _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 20,
                        Window = TimeSpan.FromMinutes(5),
                        QueueLimit = 2
                    });
            });

            // Admin endpoints — per-user, generous
            options.AddPolicy("admin", context =>
            {
                var userId = context.User?.FindFirst("sub")?.Value
                             ?? context.Connection.RemoteIpAddress?.ToString()
                             ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(userId, _ =>
                    new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 300,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 5
                    });
            });

            // Global fallback — per-IP, generous
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 600,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 20
                    }));

            options.RejectionStatusCode = 429;
        });

        return services;
    }
}