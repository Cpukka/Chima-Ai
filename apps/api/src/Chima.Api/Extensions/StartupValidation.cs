using Chima.Application.Settings;

namespace Chima.Api.Extensions;

public static class StartupValidation
{
    /// <summary>
    /// Fails fast at startup if critical configuration is missing or unsafe.
    /// Called from Program.cs BEFORE app.Run().
    /// </summary>
    public static void ValidateConfiguration(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;
        var env = builder.Environment;
        var errors = new List<string>();

        // Connection string
        var cs = config.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(cs))
            errors.Add("ConnectionStrings:Default is required.");

        // JWT secret
        var jwtSecret = config["Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(jwtSecret))
            errors.Add("Jwt:Secret is required.");
        else
        {
            if (jwtSecret.Length < 32)
                errors.Add("Jwt:Secret must be at least 32 characters.");
            if (jwtSecret.Contains("CHANGE_ME", StringComparison.OrdinalIgnoreCase)
                || jwtSecret.Contains("dev_only", StringComparison.OrdinalIgnoreCase))
            {
                if (!env.IsDevelopment())
                    errors.Add("Jwt:Secret appears to be a placeholder. Set a real secret in production.");
            }
        }

        // Issuer / Audience
        if (string.IsNullOrWhiteSpace(config["Jwt:Issuer"]))
            errors.Add("Jwt:Issuer is required.");
        if (string.IsNullOrWhiteSpace(config["Jwt:Audience"]))
            errors.Add("Jwt:Audience is required.");

        // AI provider — warn only, don't fail
        var aiKey = config["AI:ApiKey"];
        if (string.IsNullOrWhiteSpace(aiKey))
        {
            // Not fatal — app runs but AI calls will fail gracefully
        }
        else if (aiKey.StartsWith("REPLACE", StringComparison.OrdinalIgnoreCase))
        {
            if (!env.IsDevelopment())
                errors.Add("AI:ApiKey looks like a placeholder.");
        }

        // CORS origins required in production
        if (!env.IsDevelopment())
        {
            var cors = config["Cors:AllowedOrigins"];
            if (string.IsNullOrWhiteSpace(cors) || cors.Contains("localhost"))
                errors.Add("Cors:AllowedOrigins must be set to real domains in production (no localhost).");
        }

        if (errors.Count > 0)
        {
            var message = "Configuration validation failed:\n  - " + string.Join("\n  - ", errors);
            throw new InvalidOperationException(message);
        }
    }
}