using Chima.Domain.Entities;
using Chima.Domain.Enums;
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
        await SeedAdminAsync(db, logger, ct);
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

    private static async Task SeedAdminAsync(AppDbContext db, ILogger logger, CancellationToken ct)
    {
        var email = Environment.GetEnvironmentVariable("ADMIN_BOOTSTRAP_EMAIL");
        var password = Environment.GetEnvironmentVariable("ADMIN_BOOTSTRAP_PASSWORD");
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password)) return;

        email = email.Trim().ToLowerInvariant();
        var existing = await db.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
        if (existing is not null)
        {
            var hasAdmin = await db.AdminUsers.AnyAsync(a => a.UserId == existing.Id, ct);
            if (!hasAdmin)
            {
                db.AdminUsers.Add(new AdminUser
                {
                    UserId = existing.Id,
                    Role = AdminRole.SuperAdmin,
                    IsActive = true
                });
                await db.SaveChangesAsync(ct);
                logger.LogInformation("Upgraded existing user to SuperAdmin: {Email}", email);
            }
            return;
        }

        var user = new User
        {
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password, workFactor: 12),
            EmailVerified = true,
            EmailVerifiedAt = DateTime.UtcNow,
            Status = UserStatus.Active,
            Profile = new UserProfile { DisplayName = "Admin" }
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);
        logger.LogInformation("Bootstrap admin user created: {Email}", email);
    }
}
