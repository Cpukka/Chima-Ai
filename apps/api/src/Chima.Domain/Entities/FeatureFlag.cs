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
