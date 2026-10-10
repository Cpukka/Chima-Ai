# =====================================================================
#  CHIMA — PHASE 2 SETUP SCRIPT
#  Creates Domain + Application + Infrastructure + API foundation
# =====================================================================

$ErrorActionPreference = "Stop"
$root = "C:\Users\Chima\chima\apps\api"

if (-not (Test-Path $root)) {
    Write-Host "ERROR: $root does not exist. Run from C:\Users\Chima\chima." -ForegroundColor Red
    exit 1
}

Set-Location $root

function Write-File($path, $content) {
    $full = Join-Path $root $path
    $dir = Split-Path $full -Parent
    if (-not (Test-Path $dir)) { New-Item -ItemType Directory -Force -Path $dir | Out-Null }
    Set-Content -Path $full -Value $content -Encoding UTF8
    Write-Host "  wrote $path" -ForegroundColor DarkGray
}

Write-Host "=== Phase 2: Creating folders ===" -ForegroundColor Cyan
$dirs = @(
  "src\Chima.Domain\Common",
  "src\Chima.Domain\Entities",
  "src\Chima.Domain\Enums",
  "src\Chima.Application\Common",
  "src\Chima.Infrastructure\Persistence",
  "src\Chima.Infrastructure\Persistence\Configurations",
  "src\Chima.Infrastructure\Persistence\Seed",
  "src\Chima.Api\Middleware",
  "src\Chima.Api\Filters",
  "src\Chima.Api\Extensions"
)
foreach ($d in $dirs) { New-Item -ItemType Directory -Force -Path (Join-Path $root $d) | Out-Null }

# Remove template stubs
Remove-Item "src\Chima.Domain\Class1.cs" -ErrorAction SilentlyContinue
Remove-Item "src\Chima.Application\Class1.cs" -ErrorAction SilentlyContinue
Remove-Item "src\Chima.Infrastructure\Class1.cs" -ErrorAction SilentlyContinue

Write-Host "=== Phase 2: Directory.Build.props ===" -ForegroundColor Cyan
$props = @'
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
    <TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <GenerateDocumentationFile>false</GenerateDocumentationFile>
    <InvariantGlobalization>false</InvariantGlobalization>
    <UserSecretsId>chima-api-secrets</UserSecretsId>
  </PropertyGroup>
  <PropertyGroup>
    <Authors>Chima</Authors>
    <Company>Chima</Company>
    <Product>Chima AI Assistant</Product>
    <Version>0.1.0</Version>
  </PropertyGroup>
</Project>
'@
Set-Content -Path (Join-Path $root "Directory.Build.props") -Value $props -Encoding UTF8

Write-Host "=== Phase 2: Domain layer ===" -ForegroundColor Cyan

Write-File "src\Chima.Domain\Common\BaseEntity.cs" @'
namespace Chima.Domain.Common;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
'@

Write-File "src\Chima.Domain\Enums\Enums.cs" @'
namespace Chima.Domain.Enums;

public enum UserStatus { Active = 0, Suspended = 1, Deleted = 2 }
public enum MessageRole { System = 0, User = 1, Assistant = 2 }
public enum MessageStatus { Pending = 0, Streaming = 1, Completed = 2, Failed = 3, Cancelled = 4 }
public enum SubscriptionStatus { Active = 0, Trialing = 1, PastDue = 2, Cancelled = 3, Expired = 4, Refunded = 5 }
public enum PaymentStatus { Pending = 0, Succeeded = 1, Failed = 2, Refunded = 3, Disputed = 4 }
public enum PaymentProvider { GooglePlay = 0, Stripe = 1, Paystack = 2, Manual = 99 }
public enum CreditTransactionType { Grant = 0, Purchase = 1, Consume = 2, Refund = 3, Expire = 4 }
public enum FileProcessingStatus { Pending = 0, Extracting = 1, Chunking = 2, Embedding = 3, Ready = 4, Failed = 5 }
public enum NotificationType { System = 0, Subscription = 1, UsageWarning = 2, SecurityAlert = 3, Announcement = 4 }
public enum AdminRole { SuperAdmin = 0, Admin = 1, Support = 2, Analyst = 3 }
public enum ModelCapability { Text = 0, Vision = 1, FileAnalysis = 2, Reasoning = 3, ImageGeneration = 4, Embeddings = 5 }
public enum ReportStatus { Open = 0, InReview = 1, Resolved = 2, Dismissed = 3 }
'@

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

    public UserProfile? Profile { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
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

Write-File "src\Chima.Domain\Entities\UserProfile.cs" @'
using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class UserProfile : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string? DisplayName { get; set; }
    public string? AvatarUrl { get; set; }
    public string TimeZone { get; set; } = "UTC";
    public string Language { get; set; } = "en";
    public string Theme { get; set; } = "system";

    public bool NotifySubscriptionRenewal { get; set; } = true;
    public bool NotifyUsageWarning { get; set; } = true;
    public bool NotifySecurityAlert { get; set; } = true;
    public bool NotifyProductAnnouncements { get; set; } = true;
    public bool AnalyticsOptIn { get; set; } = true;
}
'@

Write-File "src\Chima.Domain\Entities\RefreshToken.cs" @'
using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedByTokenHash { get; set; }
    public string? DeviceInfo { get; set; }
    public string? IpAddress { get; set; }

    public bool IsActive => RevokedAt is null && DateTime.UtcNow < ExpiresAt;
}
'@

Write-File "src\Chima.Domain\Entities\AdminUser.cs" @'
using Chima.Domain.Common;
using Chima.Domain.Enums;

namespace Chima.Domain.Entities;

public class AdminUser : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public AdminRole Role { get; set; } = AdminRole.Support;
    public bool IsActive { get; set; } = true;
    public DateTime? LastActionAt { get; set; }
}
'@

Write-File "src\Chima.Domain\Entities\AuditLog.cs" @'
using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class AuditLog : BaseEntity
{
    public Guid? ActorUserId { get; set; }
    public string? ActorEmail { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public string? MetadataJson { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
'@

Write-File "src\Chima.Domain\Entities\Conversation.cs" @'
using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class Conversation : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Title { get; set; } = "New conversation";
    public bool IsPinned { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Guid? LastModelId { get; set; }
    public AIModel? LastModel { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
'@

Write-File "src\Chima.Domain\Entities\Message.cs" @'
using Chima.Domain.Common;
using Chima.Domain.Enums;

namespace Chima.Domain.Entities;

public class Message : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;

    public MessageRole Role { get; set; }
    public MessageStatus Status { get; set; } = MessageStatus.Completed;
    public string Content { get; set; } = string.Empty;

    public Guid? ParentMessageId { get; set; }
    public Message? ParentMessage { get; set; }
    public Guid? BranchRootId { get; set; }

    public Guid? ModelId { get; set; }
    public AIModel? Model { get; set; }
    public int? InputTokens { get; set; }
    public int? OutputTokens { get; set; }
    public decimal? EstimatedCost { get; set; }
    public int? DurationMs { get; set; }

    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }

    public ICollection<MessageAttachment> Attachments { get; set; } = new List<MessageAttachment>();
}
'@

Write-File "src\Chima.Domain\Entities\MessageAttachment.cs" @'
using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class MessageAttachment : BaseEntity
{
    public Guid MessageId { get; set; }
    public Message Message { get; set; } = null!;
    public Guid UploadedFileId { get; set; }
    public UploadedFile UploadedFile { get; set; } = null!;
}
'@

Write-File "src\Chima.Domain\Entities\AIProvider.cs" @'
using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class AIProvider : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string ApiKeySecretName { get; set; } = string.Empty;

    public ICollection<AIModel> Models { get; set; } = new List<AIModel>();
}
'@

Write-File "src\Chima.Domain\Entities\AIModel.cs" @'
using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class AIModel : BaseEntity
{
    public Guid ProviderId { get; set; }
    public AIProvider Provider { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ModelIdentifier { get; set; } = string.Empty;

    public decimal InputPricePer1M { get; set; }
    public decimal OutputPricePer1M { get; set; }
    public int ContextWindow { get; set; }
    public int MaxOutputTokens { get; set; } = 4096;

    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }
    public bool IsPremium { get; set; }

    public string CapabilitiesCsv { get; set; } = "Text";
}
'@

Write-File "src\Chima.Domain\Entities\UsageRecord.cs" @'
using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class UsageRecord : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? ConversationId { get; set; }
    public Guid? MessageId { get; set; }
    public Guid ModelId { get; set; }
    public AIModel Model { get; set; } = null!;

    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public int TotalTokens { get; set; }
    public decimal EstimatedCost { get; set; }
    public int DurationMs { get; set; }

    public string? RequestId { get; set; }
    public bool Success { get; set; }
    public string? ErrorCode { get; set; }
}
'@

Write-File "src\Chima.Domain\Entities\UsageLimit.cs" @'
using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class UsageLimit : BaseEntity
{
    public Guid PlanId { get; set; }
    public SubscriptionPlan Plan { get; set; } = null!;

    public int MonthlyMessages { get; set; }
    public int MonthlyFileUploads { get; set; }
    public int MonthlyTokens { get; set; }
    public int MaxFileSizeMb { get; set; } = 25;
    public int MaxConversations { get; set; } = -1;
    public decimal MonthlyCreditAllowance { get; set; }
}
'@

Write-File "src\Chima.Domain\Entities\SubscriptionPlan.cs" @'
using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class SubscriptionPlan : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; }
    public int SortOrder { get; set; }

    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    public string Currency { get; set; } = "USD";

    public string? GooglePlayMonthlyProductId { get; set; }
    public string? GooglePlayYearlyProductId { get; set; }
    public string? StripeMonthlyPriceId { get; set; }
    public string? StripeYearlyPriceId { get; set; }
    public string? PaystackMonthlyPlanCode { get; set; }
    public string? PaystackYearlyPlanCode { get; set; }

    public UsageLimit? UsageLimit { get; set; }
    public ICollection<UserSubscription> Subscriptions { get; set; } = new List<UserSubscription>();
}
'@

Write-File "src\Chima.Domain\Entities\UserSubscription.cs" @'
using Chima.Domain.Common;
using Chima.Domain.Enums;

namespace Chima.Domain.Entities;

public class UserSubscription : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid PlanId { get; set; }
    public SubscriptionPlan Plan { get; set; } = null!;

    public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;
    public DateTime CurrentPeriodStart { get; set; }
    public DateTime CurrentPeriodEnd { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? EndedAt { get; set; }

    public PaymentProvider Provider { get; set; }
    public string? ExternalSubscriptionId { get; set; }
    public string? ExternalPurchaseToken { get; set; }
    public bool AutoRenew { get; set; } = true;
    public DateTime? GracePeriodEndsAt { get; set; }
}
'@

Write-File "src\Chima.Domain\Entities\Payment.cs" @'
using Chima.Domain.Common;
using Chima.Domain.Enums;

namespace Chima.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid? SubscriptionId { get; set; }
    public UserSubscription? Subscription { get; set; }

    public PaymentProvider Provider { get; set; }
    public string? ExternalPaymentId { get; set; }
    public string? ExternalOrderId { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public DateTime? PaidAt { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? ReceiptUrl { get; set; }
    public string? RawEventJson { get; set; }
}
'@

Write-File "src\Chima.Domain\Entities\CreditTransaction.cs" @'
using Chima.Domain.Common;
using Chima.Domain.Enums;

namespace Chima.Domain.Entities;

public class CreditTransaction : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public CreditTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? Reason { get; set; }
    public string? ReferenceId { get; set; }
}
'@

Write-File "src\Chima.Domain\Entities\UploadedFile.cs" @'
using Chima.Domain.Common;
using Chima.Domain.Enums;

namespace Chima.Domain.Entities;

public class UploadedFile : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string StorageKey { get; set; } = string.Empty;
    public string? Sha256 { get; set; }

    public FileProcessingStatus ProcessingStatus { get; set; } = FileProcessingStatus.Pending;
    public string? ProcessingError { get; set; }
    public DateTime? ProcessedAt { get; set; }

    public int? ExtractedCharCount { get; set; }
    public int? ChunkCount { get; set; }
    public int? PageCount { get; set; }

    public ICollection<FileChunk> Chunks { get; set; } = new List<FileChunk>();
}
'@

Write-File "src\Chima.Domain\Entities\FileChunk.cs" @'
using Chima.Domain.Common;
using Pgvector;

namespace Chima.Domain.Entities;

public class FileChunk : BaseEntity
{
    public Guid FileId { get; set; }
    public UploadedFile File { get; set; } = null!;
    public int ChunkIndex { get; set; }
    public string Content { get; set; } = string.Empty;
    public int TokenCount { get; set; }
    public int? PageNumber { get; set; }

    public Vector? Embedding { get; set; }
    public string? EmbeddingModel { get; set; }
}
'@

Write-File "src\Chima.Domain\Entities\Notification.cs" @'
using Chima.Domain.Common;
using Chima.Domain.Enums;

namespace Chima.Domain.Entities;

public class Notification : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public NotificationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string? ActionUrl { get; set; }
    public string? MetadataJson { get; set; }

    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public DateTime? SentAt { get; set; }
}
'@

Write-File "src\Chima.Domain\Entities\DeviceToken.cs" @'
using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class DeviceToken : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
    public string Platform { get; set; } = "android";
    public bool IsActive { get; set; } = true;
    public DateTime? LastUsedAt { get; set; }
}
'@

Write-File "src\Chima.Domain\Entities\Report.cs" @'
using Chima.Domain.Common;
using Chima.Domain.Enums;

namespace Chima.Domain.Entities;

public class Report : BaseEntity
{
    public Guid ReporterUserId { get; set; }
    public Guid? MessageId { get; set; }
    public Guid? ConversationId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    public ReportStatus Status { get; set; } = ReportStatus.Open;
    public string? Resolution { get; set; }
    public Guid? ResolvedByAdminId { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
'@

Write-File "src\Chima.Domain\Entities\Feedback.cs" @'
using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class Feedback : BaseEntity
{
    public Guid? UserId { get; set; }
    public string Category { get; set; } = "general";
    public string Body { get; set; } = string.Empty;
    public int? Rating { get; set; }
    public string? ContactEmail { get; set; }
    public string? MetadataJson { get; set; }
    public bool IsReviewed { get; set; }
}
'@

Write-File "src\Chima.Domain\Entities\SystemSetting.cs" @'
using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class SystemSetting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ValueType { get; set; } = "string";
    public string? Description { get; set; }
    public bool IsSecret { get; set; }
}
'@

Write-File "src\Chima.Domain\Entities\FeatureFlag.cs" @'
using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class FeatureFlag : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
    public string? EnabledForPlansCsv { get; set; }
    public int RolloutPercent { get; set; } = 100;
}
'@

Write-Host "=== Phase 2: Application layer ===" -ForegroundColor Cyan

Write-File "src\Chima.Application\Common\ApiResponse.cs" @'
namespace Chima.Application.Common;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<ApiError> Errors { get; init; } = Array.Empty<ApiError>();

    public static ApiResponse<T> Ok(T data, string? message = null) => new()
    {
        Success = true,
        Data = data,
        Message = message
    };

    public static ApiResponse<T> Fail(string message, params ApiError[] errors) => new()
    {
        Success = false,
        Message = message,
        Errors = errors
    };
}

public class ApiResponse : ApiResponse<object> { }

public record ApiError(string Code, string Message, string? Field = null);
'@

Write-Host "=== Phase 2: Infrastructure layer ===" -ForegroundColor Cyan

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
}
'@

# --- Entity configurations ---

Write-File "src\Chima.Infrastructure\Persistence\Configurations\UserConfiguration.cs" @'
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chima.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("users");
        b.HasKey(x => x.Id);
        b.Property(x => x.Email).HasMaxLength(320).IsRequired();
        b.HasIndex(x => x.Email).IsUnique();
        b.Property(x => x.PasswordHash).HasMaxLength(255).IsRequired();
        b.Property(x => x.Status).HasConversion<int>();
        b.Property(x => x.GoogleSubjectId).HasMaxLength(128);
        b.HasIndex(x => x.GoogleSubjectId);

        b.HasOne(x => x.Profile)
            .WithOne(p => p.User)
            .HasForeignKey<UserProfile>(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> b)
    {
        b.ToTable("user_profiles");
        b.HasKey(x => x.Id);
        b.Property(x => x.DisplayName).HasMaxLength(120);
        b.Property(x => x.AvatarUrl).HasMaxLength(500);
        b.Property(x => x.TimeZone).HasMaxLength(64);
        b.Property(x => x.Language).HasMaxLength(16);
        b.Property(x => x.Theme).HasMaxLength(16);
        b.HasIndex(x => x.UserId).IsUnique();
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("refresh_tokens");
        b.HasKey(x => x.Id);
        b.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
        b.HasIndex(x => x.TokenHash).IsUnique();
        b.Property(x => x.DeviceInfo).HasMaxLength(500);
        b.Property(x => x.IpAddress).HasMaxLength(64);
        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.ExpiresAt);

        b.HasOne(x => x.User).WithMany(u => u.RefreshTokens)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class AdminUserConfiguration : IEntityTypeConfiguration<AdminUser>
{
    public void Configure(EntityTypeBuilder<AdminUser> b)
    {
        b.ToTable("admin_users");
        b.HasKey(x => x.Id);
        b.Property(x => x.Role).HasConversion<int>();
        b.HasIndex(x => x.UserId).IsUnique();
        b.HasOne(x => x.User).WithMany().HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> b)
    {
        b.ToTable("audit_logs");
        b.HasKey(x => x.Id);
        b.Property(x => x.Action).HasMaxLength(120).IsRequired();
        b.Property(x => x.EntityType).HasMaxLength(120);
        b.Property(x => x.EntityId).HasMaxLength(120);
        b.Property(x => x.ActorEmail).HasMaxLength(320);
        b.Property(x => x.IpAddress).HasMaxLength(64);
        b.Property(x => x.UserAgent).HasMaxLength(500);
        b.HasIndex(x => x.CreatedAt);
        b.HasIndex(x => x.ActorUserId);
    }
}
'@

Write-File "src\Chima.Infrastructure\Persistence\Configurations\ConversationConfiguration.cs" @'
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chima.Infrastructure.Persistence.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> b)
    {
        b.ToTable("conversations");
        b.HasKey(x => x.Id);
        b.Property(x => x.Title).HasMaxLength(200).IsRequired();
        b.HasIndex(x => new { x.UserId, x.DeletedAt });
        b.HasIndex(x => new { x.UserId, x.UpdatedAt });
        b.HasIndex(x => x.IsPinned);

        b.HasOne(x => x.User).WithMany(u => u.Conversations)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.LastModel).WithMany()
            .HasForeignKey(x => x.LastModelId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> b)
    {
        b.ToTable("messages");
        b.HasKey(x => x.Id);
        b.Property(x => x.Content).IsRequired();
        b.Property(x => x.Role).HasConversion<int>();
        b.Property(x => x.Status).HasConversion<int>();
        b.Property(x => x.ErrorCode).HasMaxLength(120);
        b.HasIndex(x => new { x.ConversationId, x.CreatedAt });
        b.HasIndex(x => x.Status);

        b.HasOne(x => x.Conversation).WithMany(c => c.Messages)
            .HasForeignKey(x => x.ConversationId).OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.ParentMessage).WithMany()
            .HasForeignKey(x => x.ParentMessageId).OnDelete(DeleteBehavior.NoAction);

        b.HasOne(x => x.Model).WithMany()
            .HasForeignKey(x => x.ModelId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class MessageAttachmentConfiguration : IEntityTypeConfiguration<MessageAttachment>
{
    public void Configure(EntityTypeBuilder<MessageAttachment> b)
    {
        b.ToTable("message_attachments");
        b.HasKey(x => x.Id);
        b.HasIndex(x => x.MessageId);
        b.HasIndex(x => x.UploadedFileId);

        b.HasOne(x => x.Message).WithMany(m => m.Attachments)
            .HasForeignKey(x => x.MessageId).OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.UploadedFile).WithMany()
            .HasForeignKey(x => x.UploadedFileId).OnDelete(DeleteBehavior.Restrict);
    }
}
'@

Write-File "src\Chima.Infrastructure\Persistence\Configurations\AIConfiguration.cs" @'
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chima.Infrastructure.Persistence.Configurations;

public class AIProviderConfiguration : IEntityTypeConfiguration<AIProvider>
{
    public void Configure(EntityTypeBuilder<AIProvider> b)
    {
        b.ToTable("ai_providers");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(64).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();
        b.Property(x => x.DisplayName).HasMaxLength(120);
        b.Property(x => x.BaseUrl).HasMaxLength(500);
        b.Property(x => x.ApiKeySecretName).HasMaxLength(120);
    }
}

public class AIModelConfiguration : IEntityTypeConfiguration<AIModel>
{
    public void Configure(EntityTypeBuilder<AIModel> b)
    {
        b.ToTable("ai_models");
        b.HasKey(x => x.Id);
        b.Property(x => x.Name).HasMaxLength(64).IsRequired();
        b.HasIndex(x => x.Name).IsUnique();
        b.Property(x => x.DisplayName).HasMaxLength(120);
        b.Property(x => x.ModelIdentifier).HasMaxLength(120).IsRequired();
        b.Property(x => x.InputPricePer1M).HasPrecision(18, 6);
        b.Property(x => x.OutputPricePer1M).HasPrecision(18, 6);
        b.Property(x => x.CapabilitiesCsv).HasMaxLength(255);
        b.HasIndex(x => x.IsDefault);
        b.HasIndex(x => x.IsActive);

        b.HasOne(x => x.Provider).WithMany(p => p.Models)
            .HasForeignKey(x => x.ProviderId).OnDelete(DeleteBehavior.Restrict);
    }
}
'@

Write-File "src\Chima.Infrastructure\Persistence\Configurations\UsageConfiguration.cs" @'
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chima.Infrastructure.Persistence.Configurations;

public class UsageRecordConfiguration : IEntityTypeConfiguration<UsageRecord>
{
    public void Configure(EntityTypeBuilder<UsageRecord> b)
    {
        b.ToTable("usage_records");
        b.HasKey(x => x.Id);
        b.Property(x => x.EstimatedCost).HasPrecision(18, 6);
        b.HasIndex(x => new { x.UserId, x.CreatedAt });
        b.HasIndex(x => x.ModelId);
        b.HasIndex(x => x.RequestId);

        b.HasOne(x => x.User).WithMany(u => u.UsageRecords)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Model).WithMany()
            .HasForeignKey(x => x.ModelId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class UsageLimitConfiguration : IEntityTypeConfiguration<UsageLimit>
{
    public void Configure(EntityTypeBuilder<UsageLimit> b)
    {
        b.ToTable("usage_limits");
        b.HasKey(x => x.Id);
        b.Property(x => x.MonthlyCreditAllowance).HasPrecision(18, 6);
        b.HasIndex(x => x.PlanId).IsUnique();

        b.HasOne(x => x.Plan).WithOne(p => p.UsageLimit)
            .HasForeignKey<UsageLimit>(x => x.PlanId).OnDelete(DeleteBehavior.Cascade);
    }
}
'@

Write-File "src\Chima.Infrastructure\Persistence\Configurations\BillingConfiguration.cs" @'
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chima.Infrastructure.Persistence.Configurations;

public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
{
    public void Configure(EntityTypeBuilder<SubscriptionPlan> b)
    {
        b.ToTable("subscription_plans");
        b.HasKey(x => x.Id);
        b.Property(x => x.Code).HasMaxLength(32).IsRequired();
        b.HasIndex(x => x.Code).IsUnique();
        b.Property(x => x.DisplayName).HasMaxLength(120).IsRequired();
        b.Property(x => x.Description).HasMaxLength(500);
        b.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        b.Property(x => x.MonthlyPrice).HasPrecision(18, 2);
        b.Property(x => x.YearlyPrice).HasPrecision(18, 2);
    }
}

public class UserSubscriptionConfiguration : IEntityTypeConfiguration<UserSubscription>
{
    public void Configure(EntityTypeBuilder<UserSubscription> b)
    {
        b.ToTable("user_subscriptions");
        b.HasKey(x => x.Id);
        b.Property(x => x.Status).HasConversion<int>();
        b.Property(x => x.Provider).HasConversion<int>();
        b.Property(x => x.ExternalSubscriptionId).HasMaxLength(200);
        b.Property(x => x.ExternalPurchaseToken).HasMaxLength(1000);
        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.ExternalSubscriptionId);

        b.HasOne(x => x.User).WithMany(u => u.Subscriptions)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        b.HasOne(x => x.Plan).WithMany(p => p.Subscriptions)
            .HasForeignKey(x => x.PlanId).OnDelete(DeleteBehavior.Restrict);
    }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> b)
    {
        b.ToTable("payments");
        b.HasKey(x => x.Id);
        b.Property(x => x.Provider).HasConversion<int>();
        b.Property(x => x.Status).HasConversion<int>();
        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.Currency).HasMaxLength(3).IsRequired();
        b.Property(x => x.ExternalPaymentId).HasMaxLength(200);
        b.Property(x => x.ExternalOrderId).HasMaxLength(200);
        b.Property(x => x.ReceiptUrl).HasMaxLength(500);
        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.ExternalPaymentId);

        b.HasOne(x => x.User).WithMany(u => u.Payments)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Subscription).WithMany()
            .HasForeignKey(x => x.SubscriptionId).OnDelete(DeleteBehavior.SetNull);
    }
}

public class CreditTransactionConfiguration : IEntityTypeConfiguration<CreditTransaction>
{
    public void Configure(EntityTypeBuilder<CreditTransaction> b)
    {
        b.ToTable("credit_transactions");
        b.HasKey(x => x.Id);
        b.Property(x => x.Type).HasConversion<int>();
        b.Property(x => x.Amount).HasPrecision(18, 6);
        b.Property(x => x.BalanceAfter).HasPrecision(18, 6);
        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.CreatedAt);

        b.HasOne(x => x.User).WithMany(u => u.CreditTransactions)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
'@

Write-File "src\Chima.Infrastructure\Persistence\Configurations\FileConfiguration.cs" @'
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chima.Infrastructure.Persistence.Configurations;

public class UploadedFileConfiguration : IEntityTypeConfiguration<UploadedFile>
{
    public void Configure(EntityTypeBuilder<UploadedFile> b)
    {
        b.ToTable("uploaded_files");
        b.HasKey(x => x.Id);
        b.Property(x => x.OriginalFileName).HasMaxLength(500).IsRequired();
        b.Property(x => x.ContentType).HasMaxLength(200).IsRequired();
        b.Property(x => x.StorageKey).HasMaxLength(500).IsRequired();
        b.Property(x => x.Sha256).HasMaxLength(64);
        b.Property(x => x.ProcessingStatus).HasConversion<int>();
        b.HasIndex(x => x.UserId);
        b.HasIndex(x => x.ProcessingStatus);

        b.HasOne(x => x.User).WithMany(u => u.Files)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class FileChunkConfiguration : IEntityTypeConfiguration<FileChunk>
{
    public void Configure(EntityTypeBuilder<FileChunk> b)
    {
        b.ToTable("file_chunks");
        b.HasKey(x => x.Id);
        b.Property(x => x.Content).IsRequired();
        b.Property(x => x.EmbeddingModel).HasMaxLength(120);
        b.Property(x => x.Embedding).HasColumnType("vector(1536)");
        b.HasIndex(x => new { x.FileId, x.ChunkIndex }).IsUnique();

        b.HasOne(x => x.File).WithMany(f => f.Chunks)
            .HasForeignKey(x => x.FileId).OnDelete(DeleteBehavior.Cascade);

        // IVFFlat index — created in a later migration once data exists
    }
}
'@

Write-File "src\Chima.Infrastructure\Persistence\Configurations\MiscConfiguration.cs" @'
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Chima.Infrastructure.Persistence.Configurations;

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> b)
    {
        b.ToTable("notifications");
        b.HasKey(x => x.Id);
        b.Property(x => x.Type).HasConversion<int>();
        b.Property(x => x.Title).HasMaxLength(200).IsRequired();
        b.Property(x => x.Body).HasMaxLength(2000).IsRequired();
        b.Property(x => x.ActionUrl).HasMaxLength(500);
        b.HasIndex(x => new { x.UserId, x.IsRead });
        b.HasIndex(x => x.CreatedAt);

        b.HasOne(x => x.User).WithMany(u => u.Notifications)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class DeviceTokenConfiguration : IEntityTypeConfiguration<DeviceToken>
{
    public void Configure(EntityTypeBuilder<DeviceToken> b)
    {
        b.ToTable("device_tokens");
        b.HasKey(x => x.Id);
        b.Property(x => x.Token).HasMaxLength(500).IsRequired();
        b.HasIndex(x => x.Token).IsUnique();
        b.HasIndex(x => x.UserId);
        b.HasOne(x => x.User).WithMany(u => u.DeviceTokens)
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

public class ReportConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> b)
    {
        b.ToTable("reports");
        b.HasKey(x => x.Id);
        b.Property(x => x.Reason).HasMaxLength(200).IsRequired();
        b.Property(x => x.Details).HasMaxLength(2000);
        b.Property(x => x.Resolution).HasMaxLength(2000);
        b.Property(x => x.Status).HasConversion<int>();
        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.ReporterUserId);
    }
}

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> b)
    {
        b.ToTable("feedback");
        b.HasKey(x => x.Id);
        b.Property(x => x.Category).HasMaxLength(64).IsRequired();
        b.Property(x => x.Body).HasMaxLength(5000).IsRequired();
        b.Property(x => x.ContactEmail).HasMaxLength(320);
        b.HasIndex(x => x.CreatedAt);
    }
}

public class SystemSettingConfiguration : IEntityTypeConfiguration<SystemSetting>
{
    public void Configure(EntityTypeBuilder<SystemSetting> b)
    {
        b.ToTable("system_settings");
        b.HasKey(x => x.Id);
        b.Property(x => x.Key).HasMaxLength(120).IsRequired();
        b.HasIndex(x => x.Key).IsUnique();
        b.Property(x => x.ValueType).HasMaxLength(32);
        b.Property(x => x.Description).HasMaxLength(500);
    }
}

public class FeatureFlagConfiguration : IEntityTypeConfiguration<FeatureFlag>
{
    public void Configure(EntityTypeBuilder<FeatureFlag> b)
    {
        b.ToTable("feature_flags");
        b.HasKey(x => x.Id);
        b.Property(x => x.Key).HasMaxLength(120).IsRequired();
        b.HasIndex(x => x.Key).IsUnique();
        b.Property(x => x.DisplayName).HasMaxLength(200);
        b.Property(x => x.Description).HasMaxLength(500);
        b.Property(x => x.EnabledForPlansCsv).HasMaxLength(255);
    }
}
'@

Write-Host "=== Phase 2: Seeder ===" -ForegroundColor Cyan

Write-File "src\Chima.Infrastructure\Persistence\Seed\DbSeeder.cs" @'
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Chima.Infrastructure.Persistence.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, ILogger logger, CancellationToken ct = default)
    {
        await SeedPlansAsync(db, logger, ct);
        await SeedAIAsync(db, logger, ct);
        await SeedFeatureFlagsAsync(db, logger, ct);
        await SeedSystemSettingsAsync(db, logger, ct);
    }

    private static async Task SeedPlansAsync(AppDbContext db, ILogger logger, CancellationToken ct)
    {
        if (await db.SubscriptionPlans.AnyAsync(ct)) return;

        var plans = new[]
        {
            new SubscriptionPlan
            {
                Code = "free", DisplayName = "Free", SortOrder = 0, IsDefault = true,
                Description = "Get started with Chima. Limited monthly usage.",
                MonthlyPrice = 0, YearlyPrice = 0, Currency = "USD",
                UsageLimit = new UsageLimit
                {
                    MonthlyMessages = 50, MonthlyFileUploads = 3, MonthlyTokens = 100_000,
                    MaxFileSizeMb = 10, MaxConversations = 20, MonthlyCreditAllowance = 0
                }
            },
            new SubscriptionPlan
            {
                Code = "basic", DisplayName = "Basic", SortOrder = 1,
                Description = "For regular personal use.",
                MonthlyPrice = 4.99m, YearlyPrice = 49.99m, Currency = "USD",
                UsageLimit = new UsageLimit
                {
                    MonthlyMessages = 500, MonthlyFileUploads = 25, MonthlyTokens = 1_000_000,
                    MaxFileSizeMb = 25, MaxConversations = -1, MonthlyCreditAllowance = 5
                }
            },
            new SubscriptionPlan
            {
                Code = "pro", DisplayName = "Pro", SortOrder = 2,
                Description = "Best for power users.",
                MonthlyPrice = 14.99m, YearlyPrice = 149.99m, Currency = "USD",
                UsageLimit = new UsageLimit
                {
                    MonthlyMessages = 2000, MonthlyFileUploads = 100, MonthlyTokens = 5_000_000,
                    MaxFileSizeMb = 50, MaxConversations = -1, MonthlyCreditAllowance = 20
                }
            },
            new SubscriptionPlan
            {
                Code = "premium", DisplayName = "Premium", SortOrder = 3,
                Description = "Highest limits, advanced models, priority processing.",
                MonthlyPrice = 29.99m, YearlyPrice = 299.99m, Currency = "USD",
                UsageLimit = new UsageLimit
                {
                    MonthlyMessages = 5000, MonthlyFileUploads = 300, MonthlyTokens = 20_000_000,
                    MaxFileSizeMb = 100, MaxConversations = -1, MonthlyCreditAllowance = 50
                }
            }
        };

        db.SubscriptionPlans.AddRange(plans);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} subscription plans.", plans.Length);
    }

    private static async Task SeedAIAsync(AppDbContext db, ILogger logger, CancellationToken ct)
    {
        if (await db.AIProviders.AnyAsync(ct)) return;

        var openai = new AIProvider
        {
            Name = "openai",
            DisplayName = "OpenAI",
            BaseUrl = "https://api.openai.com/v1",
            IsActive = true,
            ApiKeySecretName = "AI_API_KEY"
        };
        db.AIProviders.Add(openai);
        await db.SaveChangesAsync(ct);

        var models = new[]
        {
            new AIModel
            {
                ProviderId = openai.Id, Name = "gpt-4o-mini", DisplayName = "GPT-4o mini",
                ModelIdentifier = "gpt-4o-mini",
                InputPricePer1M = 0.15m, OutputPricePer1M = 0.60m,
                ContextWindow = 128_000, MaxOutputTokens = 16_384,
                CapabilitiesCsv = "Text,Vision,FileAnalysis", IsActive = true, IsDefault = true
            },
            new AIModel
            {
                ProviderId = openai.Id, Name = "gpt-4o", DisplayName = "GPT-4o",
                ModelIdentifier = "gpt-4o",
                InputPricePer1M = 2.50m, OutputPricePer1M = 10.00m,
                ContextWindow = 128_000, MaxOutputTokens = 16_384,
                CapabilitiesCsv = "Text,Vision,FileAnalysis,Reasoning",
                IsActive = true, IsPremium = true
            },
            new AIModel
            {
                ProviderId = openai.Id, Name = "text-embedding-3-small", DisplayName = "Embedding Small",
                ModelIdentifier = "text-embedding-3-small",
                InputPricePer1M = 0.02m, OutputPricePer1M = 0m,
                ContextWindow = 8191, MaxOutputTokens = 0,
                CapabilitiesCsv = "Embeddings", IsActive = true
            }
        };
        db.AIModels.AddRange(models);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Seeded AI provider OpenAI and {Count} models.", models.Length);
    }

    private static async Task SeedFeatureFlagsAsync(AppDbContext db, ILogger logger, CancellationToken ct)
    {
        if (await db.FeatureFlags.AnyAsync(ct)) return;

        var flags = new[]
        {
            new FeatureFlag { Key = "file_uploads",   DisplayName = "File uploads",   IsEnabled = true },
            new FeatureFlag { Key = "vision",         DisplayName = "Vision",          IsEnabled = true },
            new FeatureFlag { Key = "web_search",     DisplayName = "Web search",      IsEnabled = false },
            new FeatureFlag { Key = "advanced_models",DisplayName = "Advanced models", IsEnabled = true },
            new FeatureFlag { Key = "image_gen",      DisplayName = "Image generation",IsEnabled = false },
            new FeatureFlag { Key = "subscriptions",  DisplayName = "Subscriptions",   IsEnabled = true },
            new FeatureFlag { Key = "voice",          DisplayName = "Voice features",  IsEnabled = false }
        };
        db.FeatureFlags.AddRange(flags);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} feature flags.", flags.Length);
    }

    private static async Task SeedSystemSettingsAsync(AppDbContext db, ILogger logger, CancellationToken ct)
    {
        if (await db.SystemSettings.AnyAsync(ct)) return;

        var settings = new[]
        {
            new SystemSetting { Key = "app.name",                 Value = "Chima",              ValueType = "string" },
            new SystemSetting { Key = "app.support_email",        Value = "support@chima.app",  ValueType = "string" },
            new SystemSetting { Key = "maintenance.enabled",      Value = "false",              ValueType = "bool" },
            new SystemSetting { Key = "registration.enabled",     Value = "true",               ValueType = "bool" },
            new SystemSetting { Key = "ai.enabled",               Value = "true",               ValueType = "bool" },
            new SystemSetting { Key = "uploads.max_file_size_mb", Value = "25",                 ValueType = "int" }
        };
        db.SystemSettings.AddRange(settings);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Seeded {Count} system settings.", settings.Length);
    }
}
'@

Write-Host "=== Phase 2: API layer ===" -ForegroundColor Cyan

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
        catch (Exception ex)
        {
            var traceId = context.TraceIdentifier;
            _logger.LogError(ex, "Unhandled exception. TraceId={TraceId} Path={Path}", traceId, context.Request.Path);

            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.ContentType = "application/json";

            var payload = new ApiResponse
            {
                Success = false,
                Message = "An unexpected error occurred. Please try again later.",
                Errors = new[] { new ApiError("server_error", "Internal server error") }
            };

            var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            await context.Response.WriteAsync(json);
        }
    }
}
'@

Write-File "src\Chima.Api\Middleware\CorrelationIdMiddleware.cs" @'
namespace Chima.Api.Middleware;

public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-Id";
    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var id = context.Request.Headers.TryGetValue(HeaderName, out var existing)
            ? existing.ToString()
            : Guid.NewGuid().ToString("N");

        context.Items[HeaderName] = id;
        context.Response.Headers[HeaderName] = id;
        await _next(context);
    }
}
'@

Write-File "src\Chima.Api\Extensions\ServiceCollectionExtensions.cs" @'
using Chima.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

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

    public static IServiceCollection AddChimaSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Chima API",
                Version = "v1",
                Description = "Chima AI Assistant — REST API"
            });

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
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
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
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// ---- Serilog ----
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

// ---- Services ----
builder.Services.AddControllers();
builder.Services.AddChimaPersistence(builder.Configuration);
builder.Services.AddChimaSwagger();
builder.Services.AddChimaCors(builder.Configuration);

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Default")!, name: "postgres");

var app = builder.Build();

// ---- Migrate + Seed on startup ----
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

// ---- Pipeline ----
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSerilogRequestLogging();
app.UseCors("Default");

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
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = _ => true
});

app.Run();
'@

Write-File "src\Chima.Api\appsettings.json" @'
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=chima_dev;Username=chima;Password=chima_dev_password"
  },
  "Cors": {
    "AllowedOrigins": "http://localhost:3000,http://localhost:3001"
  },
  "Serilog": {
    "MinimumLevel": "Information"
  },
  "AllowedHosts": "*"
}
'@

Write-File "src\Chima.Api\appsettings.Development.json" @'
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=chima_dev;Username=chima;Password=chima_dev_password"
  },
  "Cors": {
    "AllowedOrigins": "http://localhost:3000,http://localhost:3001"
  }
}
'@

Write-Host "=== Phase 2: NuGet packages ===" -ForegroundColor Cyan

function Ensure-Package($project, $pkg, $version) {
    $csproj = Get-Content $project -Raw
    if ($csproj -notmatch [regex]::Escape($pkg)) {
        Write-Host "  adding $pkg -> $project" -ForegroundColor DarkGray
        dotnet add $project package $pkg --version $version | Out-Null
    } else {
        Write-Host "  skip  $pkg (already present)" -ForegroundColor DarkGray
    }
}

Ensure-Package "src\Chima.Api\Chima.Api.csproj" "Swashbuckle.AspNetCore" "6.9.0"
Ensure-Package "src\Chima.Application\Chima.Application.csproj" "FluentValidation" "11.10.0"
Ensure-Package "src\Chima.Application\Chima.Application.csproj" "FluentValidation.DependencyInjectionExtensions" "11.10.0"
Ensure-Package "src\Chima.Infrastructure\Chima.Infrastructure.csproj" "Microsoft.EntityFrameworkCore" "9.0.0"
Ensure-Package "src\Chima.Infrastructure\Chima.Infrastructure.csproj" "Microsoft.EntityFrameworkCore.Design" "9.0.0"
Ensure-Package "src\Chima.Infrastructure\Chima.Infrastructure.csproj" "Microsoft.EntityFrameworkCore.Relational" "9.0.0"
Ensure-Package "src\Chima.Infrastructure\Chima.Infrastructure.csproj" "Npgsql.EntityFrameworkCore.PostgreSQL" "9.0.0"
Ensure-Package "src\Chima.Infrastructure\Chima.Infrastructure.csproj" "Pgvector.EntityFrameworkCore" "0.2.0"
Ensure-Package "src\Chima.Infrastructure\Chima.Infrastructure.csproj" "BCrypt.Net-Next" "4.0.3"
Ensure-Package "src\Chima.Api\Chima.Api.csproj" "Microsoft.EntityFrameworkCore.Design" "9.0.0"
Ensure-Package "src\Chima.Api\Chima.Api.csproj" "Serilog.AspNetCore" "8.0.3"
Ensure-Package "src\Chima.Api\Chima.Api.csproj" "Serilog.Sinks.Console" "6.0.0"
Ensure-Package "src\Chima.Api\Chima.Api.csproj" "Serilog.Sinks.File" "6.0.0"
Ensure-Package "src\Chima.Api\Chima.Api.csproj" "Serilog.Enrichers.Environment" "3.0.1"
Ensure-Package "src\Chima.Api\Chima.Api.csproj" "Serilog.Enrichers.Thread" "4.0.0"
Ensure-Package "src\Chima.Api\Chima.Api.csproj" "Microsoft.AspNetCore.Authentication.JwtBearer" "9.0.0"
Ensure-Package "src\Chima.Api\Chima.Api.csproj" "AspNetCore.HealthChecks.NpgSql" "8.0.2"

Write-Host "=== Phase 2: Build ===" -ForegroundColor Cyan
dotnet restore | Out-Host
dotnet build --nologo | Out-Host

Write-Host "=== Phase 2: Create EF migration ===" -ForegroundColor Cyan
$env:ASPNETCORE_ENVIRONMENT = "Development"
dotnet ef migrations add InitialCreate `
    --project src\Chima.Infrastructure\Chima.Infrastructure.csproj `
    --startup-project src\Chima.Api\Chima.Api.csproj `
    --output-dir Persistence\Migrations | Out-Host

Write-Host ""
Write-Host "=== DONE ===" -ForegroundColor Green
Write-Host ""
Write-Host "Next: run the API to apply migration + seed:" -ForegroundColor Yellow
Write-Host "  cd $root"
Write-Host "  dotnet run --project src\Chima.Api"
Write-Host ""
Write-Host "Then in a browser:  http://localhost:5000/swagger  (or whatever URL Kestrel prints)"
Write-Host ""
