namespace Chima.Application.DTOs.Admin;

public class AdminAiProviderDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string ApiKeySecretName { get; set; } = string.Empty;
    public int ModelCount { get; set; }
}

public class CreateAiProviderRequest
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKeySecretName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class UpdateAiProviderRequest
{
    public string? DisplayName { get; set; }
    public string? BaseUrl { get; set; }
    public bool? IsActive { get; set; }
}

public class AdminAiModelDto
{
    public Guid Id { get; set; }
    public Guid ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ModelIdentifier { get; set; } = string.Empty;
    public decimal InputPricePer1M { get; set; }
    public decimal OutputPricePer1M { get; set; }
    public int ContextWindow { get; set; }
    public int MaxOutputTokens { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public bool IsPremium { get; set; }
    public string CapabilitiesCsv { get; set; } = string.Empty;
}

public class CreateAiModelRequest
{
    public Guid ProviderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ModelIdentifier { get; set; } = string.Empty;
    public decimal InputPricePer1M { get; set; }
    public decimal OutputPricePer1M { get; set; }
    public int ContextWindow { get; set; }
    public int MaxOutputTokens { get; set; } = 4096;
    public bool IsActive { get; set; } = true;
    public bool IsPremium { get; set; }
    public string CapabilitiesCsv { get; set; } = "Text";
}

public class UpdateAiModelRequest
{
    public string? DisplayName { get; set; }
    public decimal? InputPricePer1M { get; set; }
    public decimal? OutputPricePer1M { get; set; }
    public int? ContextWindow { get; set; }
    public int? MaxOutputTokens { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsPremium { get; set; }
    public string? CapabilitiesCsv { get; set; }
}