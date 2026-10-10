using Chima.Application.Interfaces;
using Chima.Application.Services;
using Chima.Application.Settings;
using Chima.Infrastructure.AI;
using Chima.Infrastructure.Authentication;
using Chima.Infrastructure.Email;
using Chima.Infrastructure.Persistence.Repositories;
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
        services.Configure<AISettings>(config.GetSection(AISettings.SectionName));

        services.AddHttpContextAccessor();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddSingleton<IEmailService, ConsoleEmailService>();
        services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();

        // Auth repos
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<IEmailVerificationTokenRepository, EmailVerificationTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Conversations
        services.AddScoped<IConversationRepository, ConversationRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();

        // AI
        services.AddScoped<IAIModelRepository, AIModelRepository>();
        services.AddScoped<IUsageRecordRepository, UsageRecordRepository>();
        services.AddScoped<IModelResolver, ModelResolver>();
        services.AddScoped<IUsageService, UsageService>();
        services.AddScoped<IAIService, AIService>();
        services.AddScoped<AIProviderFactory>();

        // Subscriptions & usage
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IUsageRepository, UsageRepository>();

        services.AddHttpClient("OpenAI", (sp, http) =>
        {
            var s = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AISettings>>().Value;
            http.BaseAddress = new Uri(s.BaseUrl);
            http.Timeout = TimeSpan.FromSeconds(s.TimeoutSeconds);
            if (!string.IsNullOrWhiteSpace(s.ApiKey))
                http.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", s.ApiKey);
        });
        services.AddScoped<OpenAIProvider>(sp =>
        {
            var s = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AISettings>>();
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<OpenAIProvider>>();
            return new OpenAIProvider(s, factory.CreateClient("OpenAI"), logger);
        });
        services.AddScoped<CompatibleAIProvider>(sp =>
        {
            var s = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<AISettings>>();
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            var logger = sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<OpenAIProvider>>();
            return new CompatibleAIProvider(s, factory, logger);
        });

        services.AddScoped<IAIProvider>(sp => sp.GetRequiredService<AIProviderFactory>().Get());
        services.AddScoped<IAIStreamProvider>(sp => (IAIStreamProvider)sp.GetRequiredService<AIProviderFactory>().Get());
        // Files & RAG
        services.AddSingleton<IFileStorageProvider, Chima.Infrastructure.Storage.LocalFileStorageProvider>();
        services.AddScoped<IDocumentExtractor, Chima.Infrastructure.Extraction.TxtExtractor>();
        services.AddScoped<IDocumentExtractor, Chima.Infrastructure.Extraction.PdfExtractor>();
        services.AddScoped<IDocumentExtractor, Chima.Infrastructure.Extraction.DocxExtractor>();
        services.AddScoped<Chima.Infrastructure.Extraction.DocumentExtractorRegistry>();
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IChunkingService, ChunkingService>();
        services.AddScoped<IEmbeddingService, EmbeddingService>();
        services.AddScoped<IRagService, RagService>();
        services.AddHostedService<Chima.Infrastructure.BackgroundServices.FileProcessingBackgroundService>();
        // Admin
        services.AddScoped<Chima.Application.Interfaces.Admin.IAuditLogRepository, Chima.Infrastructure.Persistence.Repositories.AuditLogRepository>();
        services.AddScoped<Chima.Application.Interfaces.Admin.IAdminUserRepository, Chima.Infrastructure.Persistence.Repositories.AdminUserRepository>();
        services.AddScoped<Chima.Application.Interfaces.Admin.IAdminMetricsRepository, Chima.Infrastructure.Persistence.Repositories.AdminMetricsRepository>();
        services.AddScoped<Chima.Application.Interfaces.Admin.IAuditLogService, Chima.Application.Services.Admin.AuditLogService>();
        services.AddScoped<Chima.Application.Interfaces.Admin.IAdminUserService, Chima.Infrastructure.Services.Admin.AdminUserService>();
        services.AddScoped<Chima.Application.Interfaces.Admin.IAdminMetricsService, Chima.Application.Services.Admin.AdminMetricsService>();
        services.AddScoped<Chima.Application.Interfaces.Admin.IAdminAiService, Chima.Infrastructure.Services.Admin.AdminAiService>();
        services.AddScoped<Chima.Application.Interfaces.Admin.IAdminSettingsService, Chima.Infrastructure.Services.Admin.AdminSettingsService>();        services.AddScoped<Chima.Application.Interfaces.Admin.IAdminPaymentRepository, Chima.Infrastructure.Persistence.Repositories.AdminPaymentRepository>();
        services.AddScoped<Chima.Application.Interfaces.Admin.IAdminSubscriptionRepository, Chima.Infrastructure.Persistence.Repositories.AdminSubscriptionRepository>();
        services.AddScoped<Chima.Application.Interfaces.Admin.IAdminPlanRepository, Chima.Infrastructure.Persistence.Repositories.AdminPlanRepository>();
        services.AddScoped<Chima.Application.Interfaces.Admin.IAdminListService, Chima.Application.Services.Admin.AdminListService>();




        return services;
    }
}