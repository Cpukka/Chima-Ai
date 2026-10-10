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
