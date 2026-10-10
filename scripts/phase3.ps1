# =====================================================================
#  CHIMA — PHASE 3 SETUP SCRIPT
#  Authentication: email/password + JWT + Google OAuth
# =====================================================================

$ErrorActionPreference = "Stop"
$root = "C:\Users\Chima\chima\apps\api"

if (-not (Test-Path $root)) {
    Write-Host "ERROR: $root does not exist." -ForegroundColor Red
    exit 1
}

Set-Location $root

function Write-File($path, $content) {
    $full = Join-Path $root $path
    $dir = Split-Path $full -Parent
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
    [System.IO.File]::WriteAllText($full, $content)
    Write-Host "  wrote $path" -ForegroundColor DarkGray
}

Write-Host "=== Phase 3: Creating folders ===" -ForegroundColor Cyan
$dirs = @(
  "src\Chima.Application\Interfaces",
  "src\Chima.Application\DTOs\Auth",
  "src\Chima.Application\Validators\Auth",
  "src\Chima.Application\Services",
  "src\Chima.Application\Settings",
  "src\Chima.Infrastructure\Authentication",
  "src\Chima.Infrastructure\Email",
  "src\Chima.Infrastructure\Configuration",
  "src\Chima.Api\Controllers"
)
foreach ($d in $dirs) { New-Item -ItemType Directory -Force -Path (Join-Path $root $d) | Out-Null }

# ============ DOMAIN — new entities ============

Write-Host "=== Phase 3: Domain entities ===" -ForegroundColor Cyan

Write-File "src\Chima.Domain\Entities\PasswordResetToken.cs" @'
using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class PasswordResetToken : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? ConsumedAt { get; set; }
    public string? RequestedFromIp { get; set; }

    public bool IsActive => ConsumedAt is null && DateTime.UtcNow < ExpiresAt;
}
'@

Write-File "src\Chima.Domain\Entities\EmailVerificationToken.cs" @'
using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class EmailVerificationToken : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? ConsumedAt { get; set; }

    public bool IsActive => ConsumedAt is null && DateTime.UtcNow < ExpiresAt;
}
'@

# Update User entity to add navigation collections
Write-File "src\Chima.Domain\Entities\User.cs" @'
using Chima.Domain.Common;
using Chima.Domain.Enums;

namespace Chima.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public bool EmailVerified { get; set; }
    public DateTime? EmailVerifiedAt { get; set; }
    public UserStatus Status { get; set; } = UserStatus.Active;
    public DateTime? LastLoginAt { get; set; }
    public string? GoogleSubjectId { get; set; }

    // Failed-login throttling
    public int FailedLoginCount { get; set; }
    public DateTime? LockedUntil { get; set; }

    public UserProfile? Profile { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<PasswordResetToken> PasswordResetTokens { get; set; } = new List<PasswordResetToken>();
    public ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; } = new List<EmailVerificationToken>();
    public ICollection<Conversation> Conversations { get; set; } = new List<Conversation>();
    public ICollection<UsageRecord> UsageRecords { get; set; } = new List<UsageRecord>();
    public ICollection<UploadedFile> Files { get; set; } = new List<UploadedFile>();
    public ICollection<UserSubscription> Subscriptions { get; set; } = new List<UserSubscription>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    public ICollection<CreditTransaction> CreditTransactions { get; set; } = new List<CreditTransaction>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<DeviceToken> DeviceTokens { get; set; } = new List<DeviceToken>();
}
'@

# ============ INFRASTRUCTURE — DbContext update ============

Write-File "src\Chima.Infrastructure\Persistence\AppDbContext.cs" @'
using Chima.Domain.Common;
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<UserProfile> UserProfiles => Set<UserProfile>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<EmailVerificationToken> EmailVerificationTokens => Set<EmailVerificationToken>();
    public DbSet<AdminUser> AdminUsers => Set<AdminUser>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<MessageAttachment> MessageAttachments => Set<MessageAttachment>();
    public DbSet<AIProvider> AIProviders => Set<AIProvider>();
    public DbSet<AIModel> AIModels => Set<AIModel>();
    public DbSet<UsageRecord> UsageRecords => Set<UsageRecord>();
    public DbSet<UsageLimit> UsageLimits => Set<UsageLimit>();
    public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
    public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<CreditTransaction> CreditTransactions => Set<CreditTransaction>();
    public DbSet<UploadedFile> UploadedFiles => Set<UploadedFile>();
    public DbSet<FileChunk> FileChunks => Set<FileChunk>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<DeviceToken> DeviceTokens => Set<DeviceToken>();
    public DbSet<Report> Reports => Set<Report>();
    public DbSet<Feedback> Feedbacks => Set<Feedback>();
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<FeatureFlag> FeatureFlags => Set<FeatureFlag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");
        modelBuilder.HasPostgresExtension("uuid-ossp");
        modelBuilder.HasPostgresExtension("pgcrypto");
        modelBuilder.HasPostgresExtension("pg_trgm");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Convert all entity/property names to snake_case globally
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            entity.SetTableName(ToSnakeCase(entity.GetTableName()!));

            foreach (var property in entity.GetProperties())
                property.SetColumnName(ToSnakeCase(property.GetColumnName()));

            foreach (var key in entity.GetKeys())
                key.SetName(ToSnakeCase(key.GetName()!));

            foreach (var fk in entity.GetForeignKeys())
                fk.SetConstraintName(ToSnakeCase(fk.GetConstraintName()!));

            foreach (var idx in entity.GetIndexes())
                idx.SetDatabaseName(ToSnakeCase(idx.GetDatabaseName()!));
        }

        base.OnModelCreating(modelBuilder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = DateTime.UtcNow;
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    private static string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        var sb = new System.Text.StringBuilder(input.Length + 8);
        for (int i = 0; i < input.Length; i++)
        {
            var c = input[i];
            if (char.IsUpper(c))
            {
                if (i > 0 && (!char.IsUpper(input[i - 1]) || (i + 1 < input.Length && !char.IsUpper(input[i + 1]))))
                    sb.Append('_');
                sb.Append(char.ToLowerInvariant(c));
            }
            else sb.Append(c);
        }
        return sb.ToString();
    }
}
'@

Write-File "src\Chima.Infrastructure\Persistence\Configurations\AuthTokensConfiguration.cs" @'
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chima.Infrastructure.Persistence.Configurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> b)
    {
        b.ToTable("password_reset_tokens");
        b.HasKey(x => x.Id);
        b.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
        b.HasIndex(x => x.TokenHash).IsUnique();
        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.ExpiresAt);
        b.Property(x => x.RequestedFromIp).HasMaxLength(64);

        b.HasOne(x => x.User).WithMany(u => u.PasswordResetTokens)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class EmailVerificationTokenConfiguration : IEntityTypeConfiguration<EmailVerificationToken>
{
    public void Configure(EntityTypeBuilder<EmailVerificationToken> b)
    {
        b.ToTable("email_verification_tokens");
        b.HasKey(x => x.Id);
        b.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
        b.HasIndex(x => x.TokenHash).IsUnique();
        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.ExpiresAt);

        b.HasOne(x => x.User).WithMany(u => u.EmailVerificationTokens)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}
'@

# ============ APPLICATION — settings, DTOs, interfaces, services, validators ============

Write-Host "=== Phase 3: Application layer ===" -ForegroundColor Cyan

Write-File "src\Chima.Application\Settings\JwtSettings.cs" @'
namespace Chima.Application.Settings;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "Chima";
    public string Audience { get; set; } = "ChimaClients";
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 30;
}

public class GoogleSettings
{
    public const string SectionName = "Google";

    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
}

public class AppSettings
{
    public const string SectionName = "App";

    public string FrontendBaseUrl { get; set; } = "http://localhost:3000";
}
'@

# --- DTOs ---

Write-File "src\Chima.Application\DTOs\Auth\RegisterRequest.cs" @'
namespace Chima.Application.DTOs.Auth;

public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
}
'@

Write-File "src\Chima.Application\DTOs\Auth\LoginRequest.cs" @'
namespace Chima.Application.DTOs.Auth;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? DeviceInfo { get; set; }
}
'@

Write-File "src\Chima.Application\DTOs\Auth\RefreshRequest.cs" @'
namespace Chima.Application.DTOs.Auth;

public class RefreshRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
'@

Write-File "src\Chima.Application\DTOs\Auth\ForgotPasswordRequest.cs" @'
namespace Chima.Application.DTOs.Auth;

public class ForgotPasswordRequest
{
    public string Email { get; set; } = string.Empty;
}
'@

Write-File "src\Chima.Application\DTOs\Auth\ResetPasswordRequest.cs" @'
namespace Chima.Application.DTOs.Auth;

public class ResetPasswordRequest
{
    public string Token { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
'@

Write-File "src\Chima.Application\DTOs\Auth\ChangePasswordRequest.cs" @'
namespace Chima.Application.DTOs.Auth;

public class ChangePasswordRequest
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
'@

Write-File "src\Chima.Application\DTOs\Auth\VerifyEmailRequest.cs" @'
namespace Chima.Application.DTOs.Auth;

public class VerifyEmailRequest
{
    public string Token { get; set; } = string.Empty;
}
'@

Write-File "src\Chima.Application\DTOs\Auth\AuthResponse.cs" @'
namespace Chima.Application.DTOs.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
    public UserDto User { get; set; } = new();
}

public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? DisplayName { get; set; }
    public bool EmailVerified { get; set; }
    public string? AvatarUrl { get; set; }
    public bool IsAdmin { get; set; }
}
'@

# --- Exceptions (application level) ---

Write-File "src\Chima.Application\Common\AppException.cs" @'
namespace Chima.Application.Common;

public class AppException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }

    public AppException(string code, string message, int statusCode = 400) : base(message)
    {
        Code = code;
        StatusCode = statusCode;
    }
}

public class NotFoundException : AppException
{
    public NotFoundException(string message = "Resource not found")
        : base("not_found", message, 404) { }
}

public class ValidationException : AppException
{
    public ValidationException(string message)
        : base("validation_error", message, 400) { }
}

public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "Unauthorized")
        : base("unauthorized", message, 401) { }
}

public class ConflictException : AppException
{
    public ConflictException(string message)
        : base("conflict", message, 409) { }
}

public class RateLimitException : AppException
{
    public RateLimitException(string message = "Too many requests")
        : base("rate_limited", message, 429) { }
}

public class LockedException : AppException
{
    public LockedException(string message = "Account temporarily locked")
        : base("locked", message, 423) { }
}
'@

# --- Interfaces ---

Write-File "src\Chima.Application\Interfaces\IAuthService.cs" @'
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
'@

Write-File "src\Chima.Application\Interfaces\IJwtTokenService.cs" @'
using Chima.Domain.Entities;

namespace Chima.Application.Interfaces;

public interface IJwtTokenService
{
    (string Token, int ExpiresInSeconds) CreateAccessToken(User user);
    string CreateOpaqueToken();
    string HashToken(string token);
}
'@

Write-File "src\Chima.Application\Interfaces\ICurrentUser.cs" @'
namespace Chima.Application.Interfaces;

public interface ICurrentUser
{
    Guid? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    string? IpAddress { get; }
}
'@

Write-File "src\Chima.Application\Interfaces\IEmailService.cs" @'
namespace Chima.Application.Interfaces;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string to, string displayName, string verificationLink, CancellationToken ct);
    Task SendPasswordResetEmailAsync(string to, string displayName, string resetLink, CancellationToken ct);
    Task SendWelcomeEmailAsync(string to, string displayName, CancellationToken ct);
    Task SendSecurityAlertAsync(string to, string displayName, string message, CancellationToken ct);
}
'@

Write-File "src\Chima.Application\Interfaces\IGoogleAuthService.cs" @'
namespace Chima.Application.Interfaces;

public interface IGoogleAuthService
{
    string BuildAuthorizationUrl(string state);
    Task<GoogleUserInfo> ExchangeCodeAsync(string code, CancellationToken ct);
}

public record GoogleUserInfo(string Subject, string Email, bool EmailVerified, string? Name, string? Picture);
'@

# --- Validators ---

Write-File "src\Chima.Application\Validators\Auth\AuthValidators.cs" @'
using Chima.Application.DTOs.Auth;
using FluentValidation;

namespace Chima.Application.Validators.Auth;

public static class PasswordRules
{
    public const int MinLength = 8;
    public const string Pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$";
    public const string Message = "Password must be at least 8 characters and include uppercase, lowercase, a number, and a special character.";
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(320);
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(PasswordRules.MinLength)
            .Matches(PasswordRules.Pattern).WithMessage(PasswordRules.Message);
        RuleFor(x => x.DisplayName).MaximumLength(120);
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class RefreshRequestValidator : AbstractValidator<RefreshRequest>
{
    public RefreshRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(PasswordRules.MinLength)
            .Matches(PasswordRules.Pattern).WithMessage(PasswordRules.Message);
    }
}

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(PasswordRules.MinLength)
            .Matches(PasswordRules.Pattern).WithMessage(PasswordRules.Message);
    }
}

public class VerifyEmailRequestValidator : AbstractValidator<VerifyEmailRequest>
{
    public VerifyEmailRequestValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
    }
}
'@

# ============ APPLICATION — AuthService ============

Write-File "src\Chima.Application\Services\AuthService.cs" @'
using System.Security.Cryptography;
using System.Text;
using Chima.Application.Common;
using Chima.Application.DTOs.Auth;
using Chima.Application.Interfaces;
using Chima.Application.Settings;
using Chima.Domain.Entities;
using Chima.Domain.Enums;
using Chima.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chima.Application.Services;

public class AuthService : IAuthService
{
    private const int MaxFailedLogins = 5;
    private static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);

    private readonly AppDbContext _db;
    private readonly IJwtTokenService _jwt;
    private readonly IEmailService _email;
    private readonly IGoogleAuthService _google;
    private readonly JwtSettings _jwtSettings;
    private readonly AppSettings _appSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext db,
        IJwtTokenService jwt,
        IEmailService email,
        IGoogleAuthService google,
        IOptions<JwtSettings> jwtSettings,
        IOptions<AppSettings> appSettings,
        ILogger<AuthService> logger)
    {
        _db = db;
        _jwt = jwt;
        _email = email;
        _google = google;
        _jwtSettings = jwtSettings.Value;
        _appSettings = appSettings.Value;
        _logger = logger;
    }

    // ---------- Register ----------

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, string? ip, CancellationToken ct)
    {
        var email = NormalizeEmail(request.Email);
        if (await _db.Users.AnyAsync(u => u.Email == email, ct))
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

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        // Issue verification token
        var rawVerify = _jwt.CreateOpaqueToken();
        _db.EmailVerificationTokens.Add(new EmailVerificationToken
        {
            UserId = user.Id,
            TokenHash = _jwt.HashToken(rawVerify),
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        });
        await _db.SaveChangesAsync(ct);

        var verifyLink = $"{_appSettings.FrontendBaseUrl}/verify-email?token={rawVerify}";
        await _email.SendVerificationEmailAsync(user.Email, user.Profile.DisplayName ?? "", verifyLink, ct);
        await _email.SendWelcomeEmailAsync(user.Email, user.Profile.DisplayName ?? "", ct);

        return await IssueNewSessionAsync(user, ip, ct);
    }

    // ---------- Login ----------

    public async Task<AuthResponse> LoginAsync(LoginRequest request, string? ip, CancellationToken ct)
    {
        var email = NormalizeEmail(request.Email);
        var user = await _db.Users
            .Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Email == email, ct);

        if (user is null)
            throw new UnauthorizedException("Invalid email or password.");

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
            await _db.SaveChangesAsync(ct);
            throw new UnauthorizedException("Invalid email or password.");
        }

        // Successful login — reset throttle state
        user.FailedLoginCount = 0;
        user.LockedUntil = null;
        user.LastLoginAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return await IssueNewSessionAsync(user, ip, ct);
    }

    // ---------- Refresh (rotation + reuse detection) ----------

    public async Task<AuthResponse> RefreshAsync(RefreshRequest request, string? ip, CancellationToken ct)
    {
        var hash = _jwt.HashToken(request.RefreshToken);
        var token = await _db.RefreshTokens
            .Include(t => t.User).ThenInclude(u => u.Profile)
            .FirstOrDefaultAsync(t => t.TokenHash == hash, ct);

        if (token is null)
            throw new UnauthorizedException("Invalid refresh token.");

        // Reuse detection: a revoked token being used again means theft.
        if (token.RevokedAt is not null)
        {
            _logger.LogWarning("Refresh token reuse detected for user {UserId}. Revoking all tokens.", token.UserId);
            var siblings = await _db.RefreshTokens
                .Where(t => t.UserId == token.UserId && t.RevokedAt == null)
                .ToListAsync(ct);
            foreach (var s in siblings) s.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            await _email.SendSecurityAlertAsync(
                token.User.Email, token.User.Profile?.DisplayName ?? "",
                "A refresh token was reused. All sessions have been revoked. If this wasn't you, reset your password immediately.",
                ct);
            throw new UnauthorizedException("Refresh token has been revoked. Please sign in again.");
        }

        if (!token.IsActive)
            throw new UnauthorizedException("Refresh token expired.");

        // Rotate
        token.RevokedAt = DateTime.UtcNow;

        var newRaw = _jwt.CreateOpaqueToken();
        var newHash = _jwt.HashToken(newRaw);
        token.ReplacedByTokenHash = newHash;

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = token.UserId,
            TokenHash = newHash,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
            IpAddress = ip,
            DeviceInfo = token.DeviceInfo
        });

        await _db.SaveChangesAsync(ct);

        var (access, expiresIn) = _jwt.CreateAccessToken(token.User);

        return new AuthResponse
        {
            AccessToken = access,
            RefreshToken = newRaw,
            ExpiresIn = expiresIn,
            User = MapUser(token.User)
        };
    }

    // ---------- Logout ----------

    public async Task LogoutAsync(string refreshToken, CancellationToken ct)
    {
        var hash = _jwt.HashToken(refreshToken);
        var token = await _db.RefreshTokens.FirstOrDefaultAsync(t => t.TokenHash == hash, ct);
        if (token is not null && token.RevokedAt is null)
        {
            token.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }

    // ---------- Forgot password ----------

    public async Task ForgotPasswordAsync(ForgotPasswordRequest request, string? ip, CancellationToken ct)
    {
        var email = NormalizeEmail(request.Email);
        var user = await _db.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Email == email, ct);

        // Always return success — no user enumeration
        if (user is null) return;

        // Revoke any prior active reset tokens
        var prior = await _db.PasswordResetTokens
            .Where(t => t.UserId == user.Id && t.ConsumedAt == null)
            .ToListAsync(ct);
        foreach (var t in prior) t.ConsumedAt = DateTime.UtcNow;

        var raw = _jwt.CreateOpaqueToken();
        _db.PasswordResetTokens.Add(new PasswordResetToken
        {
            UserId = user.Id,
            TokenHash = _jwt.HashToken(raw),
            ExpiresAt = DateTime.UtcNow.AddHours(1),
            RequestedFromIp = ip
        });
        await _db.SaveChangesAsync(ct);

        var link = $"{_appSettings.FrontendBaseUrl}/reset-password?token={raw}";
        await _email.SendPasswordResetEmailAsync(user.Email, user.Profile?.DisplayName ?? "", link, ct);
    }

    // ---------- Reset password ----------

    public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken ct)
    {
        var hash = _jwt.HashToken(request.Token);
        var token = await _db.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == hash, ct);

        if (token is null || !token.IsActive)
            throw new ValidationException("Invalid or expired reset token.");

        token.ConsumedAt = DateTime.UtcNow;
        token.User.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);

        // Revoke all existing sessions
        var sessions = await _db.RefreshTokens
            .Where(t => t.UserId == token.UserId && t.RevokedAt == null)
            .ToListAsync(ct);
        foreach (var s in sessions) s.RevokedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
    }

    // ---------- Change password (authenticated) ----------

    public async Task ChangePasswordAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId, ct)
                   ?? throw new UnauthorizedException();

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
            throw new ValidationException("Current password is incorrect.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword, workFactor: 12);

        // Revoke all other sessions except the caller's current one (which we can't identify here —
        // so we revoke all and force re-login on every device; safest behavior).
        var sessions = await _db.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(ct);
        foreach (var s in sessions) s.RevokedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(ct);
    }

    // ---------- Verify email ----------

    public async Task VerifyEmailAsync(VerifyEmailRequest request, CancellationToken ct)
    {
        var hash = _jwt.HashToken(request.Token);
        var token = await _db.EmailVerificationTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == hash, ct);

        if (token is null || !token.IsActive)
            throw new ValidationException("Invalid or expired verification token.");

        token.ConsumedAt = DateTime.UtcNow;
        token.User.EmailVerified = true;
        token.User.EmailVerifiedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
    }

    // ---------- Get current user ----------

    public async Task<UserDto> GetCurrentUserAsync(Guid userId, CancellationToken ct)
    {
        var user = await _db.Users.Include(u => u.Profile)
            .FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new UnauthorizedException();
        return MapUser(user);
    }

    // ---------- Google login ----------

    public async Task<AuthResponse> GoogleLoginAsync(string code, string? ip, CancellationToken ct)
    {
        var info = await _google.ExchangeCodeAsync(code, ct);
        if (!info.EmailVerified)
            throw new UnauthorizedException("Google account email is not verified.");

        var email = NormalizeEmail(info.Email);

        var user = await _db.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Email == email, ct);

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
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);
        }
        else if (string.IsNullOrEmpty(user.GoogleSubjectId))
        {
            // Link existing email account with Google
            user.GoogleSubjectId = info.Subject;
            user.EmailVerified = true;
            user.EmailVerifiedAt ??= DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
        else if (user.GoogleSubjectId != info.Subject)
        {
            throw new UnauthorizedException("Google account mismatch.");
        }

        user.LastLoginAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return await IssueNewSessionAsync(user, ip, ct);
    }

    // ---------- helpers ----------

    private async Task<AuthResponse> IssueNewSessionAsync(User user, string? ip, CancellationToken ct)
    {
        var (access, expiresIn) = _jwt.CreateAccessToken(user);

        var rawRefresh = _jwt.CreateOpaqueToken();
        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            TokenHash = _jwt.HashToken(rawRefresh),
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenDays),
            IpAddress = ip
        });
        await _db.SaveChangesAsync(ct);

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
'@

# ============ INFRASTRUCTURE — Jwt, CurrentUser, Email, Google ============

Write-Host "=== Phase 3: Infrastructure services ===" -ForegroundColor Cyan

Write-File "src\Chima.Infrastructure\Authentication\JwtTokenService.cs" @'
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Chima.Application.Interfaces;
using Chima.Application.Settings;
using Chima.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Chima.Infrastructure.Authentication;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtSettings _settings;

    public JwtTokenService(IOptions<JwtSettings> settings) => _settings = settings.Value;

    public (string Token, int ExpiresInSeconds) CreateAccessToken(User user)
    {
        var expires = DateTime.UtcNow.AddMinutes(_settings.AccessTokenMinutes);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new("ver", user.EmailVerified ? "1" : "0")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expires,
            signingCredentials: creds);

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);
        return (token, (int)(expires - DateTime.UtcNow).TotalSeconds);
    }

    public string CreateOpaqueToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(48);
        return Convert.ToBase64String(bytes)
            .Replace('+', '-').Replace('/', '_').TrimEnd('=');
    }

    public string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
'@

Write-File "src\Chima.Infrastructure\Authentication\CurrentUser.cs" @'
using System.Security.Claims;
using Chima.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Chima.Infrastructure.Authentication;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _http;

    public CurrentUser(IHttpContextAccessor http) => _http = http;

    public Guid? UserId
    {
        get
        {
            var sub = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? _http.HttpContext?.User?.FindFirstValue("sub");
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public string? Email => _http.HttpContext?.User?.FindFirstValue(ClaimTypes.Email)
                          ?? _http.HttpContext?.User?.FindFirstValue("email");

    public bool IsAuthenticated => _http.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string? IpAddress => _http.HttpContext?.Connection?.RemoteIpAddress?.ToString();
}
'@

Write-File "src\Chima.Infrastructure\Email\ConsoleEmailService.cs" @'
using Chima.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chima.Infrastructure.Email;

/// <summary>
/// Development email service — writes email content to the log.
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
'@

Write-File "src\Chima.Infrastructure\Authentication\GoogleAuthService.cs" @'
using System.Text.Json;
using Chima.Application.Interfaces;
using Chima.Application.Settings;
using Microsoft.Extensions.Options;

namespace Chima.Infrastructure.Authentication;

public class GoogleAuthService : IGoogleAuthService
{
    private const string AuthBase = "https://accounts.google.com/o/oauth2/v2/auth";
    private const string TokenEndpoint = "https://oauth2.googleapis.com/token";
    private const string UserInfoEndpoint = "https://openidconnect.googleapis.com/v1/userinfo";

    private readonly GoogleSettings _settings;
    private readonly HttpClient _http;

    public GoogleAuthService(IOptions<GoogleSettings> settings, HttpClient http)
    {
        _settings = settings.Value;
        _http = http;
    }

    public string BuildAuthorizationUrl(string state)
    {
        var q = new Dictionary<string, string?>
        {
            ["client_id"] = _settings.ClientId,
            ["redirect_uri"] = _settings.RedirectUri,
            ["response_type"] = "code",
            ["scope"] = "openid email profile",
            ["access_type"] = "offline",
            ["prompt"] = "consent",
            ["state"] = state
        };
        var qs = string.Join("&", q.Select(kv =>
            $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value ?? "")}"));
        return $"{AuthBase}?{qs}";
    }

    public async Task<GoogleUserInfo> ExchangeCodeAsync(string code, CancellationToken ct)
    {
        var form = new Dictionary<string, string>
        {
            ["code"] = code,
            ["client_id"] = _settings.ClientId,
            ["client_secret"] = _settings.ClientSecret,
            ["redirect_uri"] = _settings.RedirectUri,
            ["grant_type"] = "authorization_code"
        };

        using var tokenResp = await _http.PostAsync(TokenEndpoint, new FormUrlEncodedContent(form), ct);
        var tokenJson = await tokenResp.Content.ReadAsStringAsync(ct);
        if (!tokenResp.IsSuccessStatusCode)
            throw new UnauthorizedException($"Google token exchange failed: {tokenJson}");

        using var tokenDoc = JsonDocument.Parse(tokenJson);
        var accessToken = tokenDoc.RootElement.GetProperty("access_token").GetString()
            ?? throw new UnauthorizedException("Missing access_token from Google.");

        using var req = new HttpRequestMessage(HttpMethod.Get, UserInfoEndpoint);
        req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        using var userResp = await _http.SendAsync(req, ct);
        var userJson = await userResp.Content.ReadAsStringAsync(ct);
        if (!userResp.IsSuccessStatusCode)
            throw new UnauthorizedException($"Google userinfo fetch failed: {userJson}");

        using var userDoc = JsonDocument.Parse(userJson);
        var root = userDoc.RootElement;

        var sub = root.GetProperty("sub").GetString()!;
        var email = root.GetProperty("email").GetString()!;
        var verified = root.TryGetProperty("email_verified", out var v) && v.GetBoolean();
        var name = root.TryGetProperty("name", out var n) ? n.GetString() : null;
        var picture = root.TryGetProperty("picture", out var p) ? p.GetString() : null;

        return new GoogleUserInfo(sub, email, verified, name, picture);
    }
}
'@

Write-File "src\Chima.Infrastructure\Configuration\InfrastructureServiceExtensions.cs" @'
using Chima.Application.Interfaces;
using Chima.Application.Settings;
using Chima.Infrastructure.Authentication;
using Chima.Infrastructure.Email;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Chima.Infrastructure.Configuration;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddChimaAuthInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtSettings>(config.GetSection(JwtSettings.SectionName));
        services.Configure<GoogleSettings>(config.GetSection(GoogleSettings.SectionName));
        services.Configure<AppSettings>(config.GetSection(AppSettings.SectionName));

        services.AddHttpContextAccessor();
        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddSingleton<IEmailService, ConsoleEmailService>();
        services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();

        return services;
    }
}
'@

# ============ API — controller, exception middleware update, Program.cs ============

Write-Host "=== Phase 3: API layer ===" -ForegroundColor Cyan

Write-File "src\Chima.Api\Controllers\AuthController.cs" @'
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
'@

Write-File "src\Chima.Api\Middleware\ExceptionHandlingMiddleware.cs" @'
using System.Net;
using System.Text.Json;
using Chima.Application.Common;

namespace Chima.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            _logger.LogInformation("Application error {Code}: {Message}", ex.Code, ex.Message);
            await WriteErrorAsync(context, ex.StatusCode, ex.Code, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception. TraceId={TraceId} Path={Path}", context.TraceIdentifier, context.Request.Path);
            await WriteErrorAsync(context, 500, "server_error", "An unexpected error occurred. Please try again later.");
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, int status, string code, string message)
    {
        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        var payload = new
        {
            success = false,
            data = (object?)null,
            message,
            errors = new[] { new { code, message, field = (string?)null } }
        };

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await context.Response.WriteAsync(json);
    }
}
'@

Write-File "src\Chima.Api\Extensions\ServiceCollectionExtensions.cs" @'
using Chima.Application.Interfaces;
using Chima.Application.Services;
using Chima.Infrastructure.Configuration;
using Chima.Infrastructure.Persistence;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

namespace Chima.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddChimaPersistence(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("Default")
                 ?? throw new InvalidOperationException("Connection string 'Default' missing.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(cs, npg =>
            {
                npg.UseVector();
                npg.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            }));

        return services;
    }

    public static IServiceCollection AddChimaAuth(this IServiceCollection services, IConfiguration config)
    {
        // JWT bearer
        var jwt = config.GetSection("Jwt");
        var secret = jwt["Secret"] ?? throw new InvalidOperationException("Jwt:Secret missing.");

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt["Issuer"],
                    ValidAudience = jwt["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });

        services.AddAuthorization();

        // Infrastructure auth services
        services.AddChimaAuthInfrastructure(config);

        // Application auth service
        services.AddScoped<IAuthService, AuthService>();

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(AuthService).Assembly);

        // Rate limiter — "auth" policy for sensitive endpoints
        services.AddRateLimiter(options =>
        {
            options.AddPolicy("auth", context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0,
                        AutoReplenishment = true
                    }));

            options.RejectionStatusCode = 429;
        });

        return services;
    }

    public static IServiceCollection AddChimaSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Chima API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT access token."
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                }
            });
        });
        return services;
    }

    public static IServiceCollection AddChimaCors(this IServiceCollection services, IConfiguration config)
    {
        var origins = (config["Cors:AllowedOrigins"] ?? "http://localhost:3000,http://localhost:3001")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        services.AddCors(o => o.AddPolicy("Default", p =>
            p.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

        return services;
    }
}
'@

Write-File "src\Chima.Api\Program.cs" @'
using Chima.Api.Extensions;
using Chima.Api.Middleware;
using Chima.Infrastructure.Persistence;
using Chima.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .WriteTo.Console()
    .WriteTo.File("logs/chima-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddChimaPersistence(builder.Configuration);
builder.Services.AddChimaAuth(builder.Configuration);
builder.Services.AddChimaSwagger();
builder.Services.AddChimaCors(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Default")!, name: "postgres");

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await db.Database.MigrateAsync();
        await DbSeeder.SeedAsync(db, logger);
        logger.LogInformation("Database migrated and seeded.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration/seeding failed.");
        throw;
    }
}

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSerilogRequestLogging();
app.UseCors("Default");

app.UseAuthentication();
app.UseAuthorization();

app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chima API v1");
        c.RoutePrefix = "swagger";
    });
}

app.MapControllers();
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready", new HealthCheckOptions { Predicate = _ => true });

app.Run();
'@

Write-File "src\Chima.Api\appsettings.Development.json" @'
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=chima_dev;Username=chima;Password=chima_dev_password"
  },
  "Jwt": {
    "Secret": "CHANGE_ME_dev_only_secret_at_least_64_chars_long_0123456789abcdef",
    "Issuer": "Chima",
    "Audience": "ChimaClients",
    "AccessTokenMinutes": 15,
    "RefreshTokenDays": 30
  },
  "Google": {
    "ClientId": "REPLACE_WITH_YOUR_GOOGLE_CLIENT_ID",
    "ClientSecret": "REPLACE_WITH_YOUR_GOOGLE_CLIENT_SECRET",
    "RedirectUri": "http://localhost:5080/api/auth/google/callback"
  },
  "App": {
    "FrontendBaseUrl": "http://localhost:3000"
  },
  "Cors": {
    "AllowedOrigins": "http://localhost:3000,http://localhost:3001"
  }
}
'@

# Fix launchSettings.json — pin Kestrel to :5080
Write-File "src\Chima.Api\Properties\launchSettings.json" @'
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "Chima.Api": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": false,
      "applicationUrl": "http://localhost:5080",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  }
}
'@

# ============ Packages ============

Write-Host "=== Phase 3: NuGet packages ===" -ForegroundColor Cyan

function Ensure-Package($project, $pkg, $version) {
    $csproj = Get-Content $project -Raw
    if ($csproj -notmatch [regex]::Escape($pkg)) {
        Write-Host "  adding $pkg -> $project" -ForegroundColor DarkGray
        dotnet add $project package $pkg --version $version | Out-Null
    } else {
        Write-Host "  skip  $pkg (already present)" -ForegroundColor DarkGray
    }
}

Ensure-Package "src\Chima.Application\Chima.Application.csproj" "FluentValidation.AspNetCore" "11.3.0"
Ensure-Package "src\Chima.Application\Chima.Application.csproj" "BCrypt.Net-Next" "4.0.3"
Ensure-Package "src\Chima.Application\Chima.Application.csproj" "Microsoft.Extensions.Options" "9.0.0"
Ensure-Package "src\Chima.Application\Chima.Application.csproj" "Microsoft.Extensions.Logging.Abstractions" "9.0.0"
Ensure-Package "src\Chima.Infrastructure\Chima.Infrastructure.csproj" "System.IdentityModel.Tokens.Jwt" "8.2.0"
Ensure-Package "src\Chima.Api\Chima.Api.csproj" "System.Threading.RateLimiting" "9.0.0"

# Add ProjectReference: Application -> Infrastructure (for AppDbContext usage)
$appCsproj = "src\Chima.Application\Chima.Application.csproj"
$appXml = Get-Content $appCsproj -Raw
if ($appXml -notmatch "Chima.Infrastructure.csproj") {
    Write-Host "  adding reference Application -> Infrastructure" -ForegroundColor DarkGray
    dotnet add $appCsproj reference "src\Chima.Infrastructure\Chima.Infrastructure.csproj" | Out-Null
}

# ============ Build ============

Write-Host "=== Phase 3: Build ===" -ForegroundColor Cyan
dotnet restore | Out-Host
dotnet build --nologo | Out-Host

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed. Fix errors above before continuing." -ForegroundColor Red
    exit 1
}

# ============ Migration ============

Write-Host "=== Phase 3: Create migration (snake_case + auth tokens) ===" -ForegroundColor Cyan
$env:ASPNETCORE_ENVIRONMENT = "Development"

dotnet ef migrations add AuthAndSnakeCase `
    --project src\Chima.Infrastructure\Chima.Infrastructure.csproj `
    --startup-project src\Chima.Api\Chima.Api.csproj `
    --output-dir Persistence\Migrations | Out-Host

Write-Host ""
Write-Host "=== Phase 3 complete. ===" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "  1. Edit src\Chima.Api\appsettings.Development.json"
Write-Host "     - Paste your real Google ClientId and ClientSecret"
Write-Host "  2. Run the API:"
Write-Host "       dotnet run --project src\Chima.Api"
Write-Host "     - Migration applies automatically."
Write-Host "  3. Open http://localhost:5080/swagger"
Write-Host ""