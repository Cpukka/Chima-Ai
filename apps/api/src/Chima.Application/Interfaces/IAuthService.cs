using Chima.Application.DTOs.Auth;

namespace Chima.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, string? ip, CancellationToken ct);
    Task<AuthResponse> LoginAsync(LoginRequest request, string? ip, CancellationToken ct);
    Task<AuthResponse> RefreshAsync(RefreshRequest request, string? ip, CancellationToken ct);
    Task LogoutAsync(string refreshToken, CancellationToken ct);
    Task ForgotPasswordAsync(ForgotPasswordRequest request, string? ip, CancellationToken ct);
    Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct);
    Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct);
    Task VerifyEmailAsync(VerifyEmailRequest request, CancellationToken ct);
    Task<UserDto> GetCurrentUserAsync(Guid userId, CancellationToken ct);
    Task<AuthResponse> GoogleLoginAsync(string code, string? ip, CancellationToken ct);
}