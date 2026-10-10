using Chima.Application.Common;
using Chima.Application.DTOs.Auth;
using Chima.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Chima.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _auth;
    private readonly ICurrentUser _current;
    private readonly IGoogleAuthService _google;

    public AuthController(IAuthService auth, ICurrentUser current, IGoogleAuthService google)
    {
        _auth = auth;
        _current = current;
        _google = google;
    }

    [HttpPost("register")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request, CancellationToken ct)
    {
        var result = await _auth.RegisterAsync(request, _current.IpAddress, ct);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "Account created."));
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _auth.LoginAsync(request, _current.IpAddress, ct);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "Signed in."));
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request, CancellationToken ct)
    {
        var result = await _auth.RefreshAsync(request, _current.IpAddress, ct);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "Token refreshed."));
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshRequest request, CancellationToken ct)
    {
        await _auth.LogoutAsync(request.RefreshToken, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Signed out."));
    }

    [HttpPost("forgot-password")]
    [EnableRateLimiting("auth")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request, CancellationToken ct)
    {
        await _auth.ForgotPasswordAsync(request, _current.IpAddress, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "If an account exists for this email, a reset link was sent."));
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request, CancellationToken ct)
    {
        await _auth.ResetPasswordAsync(request, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Password has been reset. Please sign in."));
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        var userId = _current.UserId ?? throw new UnauthorizedException();
        await _auth.ChangePasswordAsync(userId, request, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Password changed. Please sign in again."));
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request, CancellationToken ct)
    {
        await _auth.VerifyEmailAsync(request, ct);
        return Ok(ApiResponse<object>.Ok(new { }, "Email verified."));
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var userId = _current.UserId ?? throw new UnauthorizedException();
        var user = await _auth.GetCurrentUserAsync(userId, ct);
        return Ok(ApiResponse<UserDto>.Ok(user));
    }

    // ---------- Google OAuth ----------

    [HttpGet("google/start")]
    public IActionResult GoogleStart()
    {
        var state = Guid.NewGuid().ToString("N");
        var url = _google.BuildAuthorizationUrl(state);
        return Ok(ApiResponse<object>.Ok(new { url, state }));
    }

    [HttpGet("google/callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code, CancellationToken ct)
    {
        var result = await _auth.GoogleLoginAsync(code, _current.IpAddress, ct);
        return Ok(ApiResponse<AuthResponse>.Ok(result, "Signed in with Google."));
    }
}