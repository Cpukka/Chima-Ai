namespace Chima.Application.Interfaces;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string to, string displayName, string verificationLink, CancellationToken ct);
    Task SendPasswordResetEmailAsync(string to, string displayName, string resetLink, CancellationToken ct);
    Task SendWelcomeEmailAsync(string to, string displayName, CancellationToken ct);
    Task SendSecurityAlertAsync(string to, string displayName, string message, CancellationToken ct);
}