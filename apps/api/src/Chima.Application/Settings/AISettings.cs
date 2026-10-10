namespace Chima.Application.Settings;

public class AISettings
{
    public const string SectionName = "AI";

    public string Provider { get; set; } = "openai";
    public string ApiKey { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = "https://api.openai.com/v1";
    public string DefaultModel { get; set; } = "gpt-4o-mini";
    public int MaxRetries { get; set; } = 2;
    public int TimeoutSeconds { get; set; } = 60;
}