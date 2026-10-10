using Chima.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chima.Infrastructure.Email;

/// <summary>
/// Development email service â€” writes email content to the log.
/// Replaced by a real SMTP/provider implementation in Phase 14.
/// </summary>
public class ConsoleEmailService : IEmailService
{
    private readonly ILogger<ConsoleEmailService> _logger;

    public ConsoleEmailService(ILogger<ConsoleEmailService> logger) => _logger = logger;

    public Task SendVerificationEmailAsync(string to, string displayName, string link, CancellationToken ct)
    {
        _logger.LogInformation("[EMAIL] Verify Email -> {To} ({Name})\n  Link: {Link}", to, displayName, link);
        return Task.CompletedTask;
    }

    public Task SendPasswordResetEmailAsync(string to, string displayName, string link, CancellationToken ct)
    {
        _logger.LogInformation("[EMAIL] Password Reset -> {To} ({Name})\n  Link: {Link}", to, displayName, link);
        return Task.CompletedTask;
    }

    public Task SendWelcomeEmailAsync(string to, string displayName, CancellationToken ct)
    {
        _logger.LogInformation("[EMAIL] Welcome -> {To} ({Name})", to, displayName);
        return Task.CompletedTask;
    }

    public Task SendSecurityAlertAsync(string to, string displayName, string message, CancellationToken ct)
    {
        _logger.LogWarning("[EMAIL] Security Alert -> {To} ({Name}): {Message}", to, displayName, message);
        return Task.CompletedTask;
    }
}