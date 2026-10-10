using System.Security.Cryptography;
using System.Text;
using Chima.Application.Common;
using Chima.Application.DTOs.Auth;
using Chima.Application.Interfaces;
using Chima.Application.Settings;
using Chima.Domain.Entities;
using Chima.Domain.Enums;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chima.Application.Services;

public class AuthService : IAuthService
{
    private const int MaxFailedLogins = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refresh;
    private readonly IPasswordResetTokenRepository _resets;
    private readonly IEmailVerificationTokenRepository _verifications;
    private readonly IUnitOfWork _uow;
    private readonly IJwtTokenService _jwt;
    private readonly IEmailService _email;
    private readonly IGoogleAuthService _google;
    private readonly JwtSettings _jwtSettings;
    private readonly AppSettings _appSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository users,
        IRefreshTokenRepository refresh,
        IPasswordResetTokenRepository resets,
        IEmailVerificationTokenRepository verifications,
        IUnitOfWork uow,
        IJwtTokenService jwt,
        IEmailService email,
        IGoogleAuthService google,
        IOptions<JwtSettings> jwtSettings,
        IOptions<AppSettings> appSettings,
        ILogger<AuthService> logger)
    {
        _users = users; _refresh = refresh; _resets = resets; _verifications = verifications;
        _uow = uow; _jwt = jwt; _email = email; _google = google;
        _jwtSettings = jwtSettings.Value; _appSettings = appSettings.Value; _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string? ip, CancellationToken ct)
    {
        var email = NormalizeEmail(request.Email);
        if (await _users.EmailExistsAsync(email, ct))
            throw new ConflictException("An account with this email already exists.");

        var user = new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password, workFactor: 12),
            EmailVerified = false,
            Status = UserStatus.Active,
            Profile = new UserProfile
            {
                DisplayName = string.IsNullOrWhiteSpace(request.DisplayName)
                    ? email.Split('@')[0]
                    : request.DisplayName!.Trim()
            }
        };

        await _users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        var rawVerify = _jwt.CreateOpaqueToken();
        await _verifications.AddAsync(new EmailVerificationToken
        {
            UserId = user.Id,
            TokenHash = _jwt.HashToken(rawVerify),
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        }, ct);
        await _uow.SaveChangesAsync(ct);

        var verifyLink = $"{_appSettings.FrontendBaseUrl}/verify-email?token={rawVerify}";
        await _email.SendVerificationEmailAsync(user.Email, user.Profile.DisplayName ?? "", verifyLink, ct);
        await _email.SendWelcomeEmailAsync(user.Email, user.Profile.DisplayName ?? "", ct);

        return await IssueNewSessionAsync(user, ip, ct);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, string? ip, CancellationToken ct)
    {
        var email = NormalizeEmail(request.Email);
        var user = await _users.FindByEmailAsync(email, ct)
                   ?? throw new UnauthorizedException("Invalid email or password.");

        if (user.Status == UserStatus.Suspended)
            throw new UnauthorizedException("This account has been suspended.");

        if (user.LockedUntil is { } until && until > DateTime.UtcNow)
            throw new LockedException($"Account locked. Try again after {until:u}.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            user.FailedLoginCount++;
            if (user.FailedLoginCount >= MaxFailedLogins)
            {
                user.LockedUntil = DateTime.UtcNow.Add(LockoutDuration);
                user.FailedLoginCount = 0;
            }
            await _uow.SaveChangesAsync(ct);
            throw new UnauthorizedException("Invalid email or password.");
        }

        user.FailedLoginCount = 0;
        user.LockedUntil = null;
        user.LastLoginAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);

        return await IssueNewSessionAsync(user, ip, ct);
    }

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request, string? ip, CancellationToken ct)
    {
        var hash = _jwt.HashToken(request.RefreshToken);
        var token = await _refresh.FindByHashAsync(hash, ct)
                    ?? throw new UnauthorizedException("Invalid refresh token.");

        if (token.RevokedAt is not null)
        {
            _logger.LogWarning("Refresh token reuse detected for user {UserId}.", token.UserId);
            await _refresh.RevokeAllForUserAsync(token.UserId, ct);
            await _uow.SaveChangesAsync(ct);
            await _email.SendSecurityAlertAsync(
                token.User.Email, token.User.Profile?.DisplayName ?? "",
                "A refresh token was reused. All sessions have been revoked. If this wasn't you, reset your password immediately.",
                ct);
            throw new UnauthorizedException("Refresh token has been revoked. Please sign in again.");
        }

        if (!token.IsActive)
            throw new UnauthorizedException("Refresh token expired.");

        token.RevokedAt = DateTime.UtcNow;

        var newRaw = _jwt.CreateOpaqueToken();
        var newHash = _jwt.HashToken(newRaw);
        token.ReplacedByTokenHash = newHash;

        await _refresh.AddAsync(new RefreshToken
        {
            UserId = token.UserId,
            TokenHash = newHash,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
            IpAddress = ip,
            DeviceInfo = token.DeviceInfo
        }, ct);
        await _uow.SaveChangesAsync(ct);

        var (access, expiresIn) = _jwt.CreateAccessToken(token.User);

        return new AuthResponse
        {
            AccessToken = access,
            RefreshToken = newRaw,
            ExpiresIn = expiresIn,
            User = MapUser(token.User)
        };
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken ct)
    {
        var hash = _jwt.HashToken(refreshToken);
        var token = await _refresh.FindByHashAsync(hash, ct);
        if (token is not null && token.RevokedAt is null)
        {
            token.RevokedAt = DateTime.UtcNow;
            await _uow.SaveChangesAsync(ct);
        }
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request, string? ip, CancellationToken ct)
    {
        var email = NormalizeEmail(request.Email);
        var user = await _users.FindByEmailAsync(email, ct);
        if (user is null) return;

        var prior = await _resets.GetActiveByUserIdAsync(user.Id, ct);
        foreach (var t in prior) t.ConsumedAt = DateTime.UtcNow;

        var raw = _jwt.CreateOpaqueToken();
        await _resets.AddAsync(new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = _jwt.HashToken(raw),
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            RequestedFromIp = ip
        }, ct);
        await _uow.SaveChangesAsync(ct);

        var link = $"{_appSettings.FrontendBaseUrl}/reset-password?token={raw}";
        await _email.SendPasswordResetEmailAsync(user.Email, user.Profile?.DisplayName ?? "", link, ct);
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct)
    {
        var hash = _jwt.HashToken(request.Token);
        var token = await _resets.FindByHashAsync(hash, ct);

        if (token is null || !token.IsActive)
            throw new ValidationException("Invalid or expired reset token.");

        token.ConsumedAt = DateTime.UtcNow;
        token.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);

        await _refresh.RevokeAllForUserAsync(token.UserId, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct)
    {
        var user = await _users.FindByIdAsync(userId, ct) ?? throw new UnauthorizedException();

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw new ValidationException("Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);

        await _refresh.RevokeAllForUserAsync(userId, ct);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task VerifyEmailAsync(VerifyEmailRequest request, CancellationToken ct)
    {
        var hash = _jwt.HashToken(request.Token);
        var token = await _verifications.FindByHashAsync(hash, ct);
        if (token is null || !token.IsActive)
            throw new ValidationException("Invalid or expired verification token.");

        token.ConsumedAt = DateTime.UtcNow;
        token.User.EmailVerified = true;
        token.User.EmailVerifiedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<UserDto> GetCurrentUserAsync(Guid userId, CancellationToken ct)
    {
        var user = await _users.FindByIdAsync(userId, ct) ?? throw new UnauthorizedException();
        return MapUser(user);
    }

    public async Task<AuthResponse> GoogleLoginAsync(string code, string? ip, CancellationToken ct)
    {
        var info = await _google.ExchangeCodeAsync(code, ct);
        if (!info.EmailVerified)
            throw new UnauthorizedException("Google account email is not verified.");

        var email = NormalizeEmail(info.Email);
        var user = await _users.FindByEmailAsync(email, ct);

        if (user is null)
        {
            user = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString("N"), workFactor: 12),
                EmailVerified = true,
                EmailVerifiedAt = DateTime.UtcNow,
                GoogleSubjectId = info.Subject,
                Profile = new UserProfile
                {
                    DisplayName = info.Name ?? email.Split('@')[0],
                    AvatarUrl = info.Picture
                }
            };
            await _users.AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);
        }
        else if (string.IsNullOrEmpty(user.GoogleSubjectId))
        {
            user.GoogleSubjectId = info.Subject;
            user.EmailVerified = true;
            user.EmailVerifiedAt ??= DateTime.UtcNow;
            await _uow.SaveChangesAsync(ct);
        }
        else if (user.GoogleSubjectId != info.Subject)
        {
            throw new UnauthorizedException("Google account mismatch.");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);

        return await IssueNewSessionAsync(user, ip, ct);
    }

    private async Task<AuthResponse> IssueNewSessionAsync(User user, string? ip, CancellationToken ct)
    {
        var (access, expiresIn) = _jwt.CreateAccessToken(user);
        var rawRefresh = _jwt.CreateOpaqueToken();

        await _refresh.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _jwt.HashToken(rawRefresh),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
            IpAddress = ip
        }, ct);
        await _uow.SaveChangesAsync(ct);

        return new AuthResponse
        {
            AccessToken = access,
            RefreshToken = rawRefresh,
            ExpiresIn = expiresIn,
            User = MapUser(user)
        };
    }

    private static UserDto MapUser(User u) => new()
    {
        Id = u.Id,
        Email = u.Email,
        DisplayName = u.Profile?.DisplayName,
        AvatarUrl = u.Profile?.AvatarUrl,
        EmailVerified = u.EmailVerified,
        IsAdmin = false
    };

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}