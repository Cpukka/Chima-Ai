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