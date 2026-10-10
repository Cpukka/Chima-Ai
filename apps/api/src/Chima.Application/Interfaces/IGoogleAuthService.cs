namespace Chima.Application.Interfaces;

public interface IGoogleAuthService
{
    string BuildAuthorizationUrl(string state);
    Task<GoogleUserInfo> ExchangeCodeAsync(string code, CancellationToken ct);
}

public record GoogleUserInfo(string Subject, string Email, bool EmailVerified, string? Name, string? Picture);