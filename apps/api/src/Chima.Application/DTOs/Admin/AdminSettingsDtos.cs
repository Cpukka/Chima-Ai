namespace Chima.Application.DTOs.Admin;

public class FeatureFlagDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsEnabled { get; set; }
    public string? EnabledForPlansCsv { get; set; }
    public int RolloutPercent { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdateFeatureFlagRequest
{
    public bool? IsEnabled { get; set; }
    public string? EnabledForPlansCsv { get; set; }
    public int? RolloutPercent { get; set; }
}

public class SystemSettingDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string ValueType { get; set; } = "string";
    public string? Description { get; set; }
    public bool IsSecret { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UpdateSystemSettingRequest
{
    public string Value { get; set; } = string.Empty;
}

public class AuditLogDto
{
    public Guid Id { get; set; }
    public Guid? ActorUserId { get; set; }
    public string? ActorEmail { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? EntityType { get; set; }
    public string? EntityId { get; set; }
    public string? MetadataJson { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; }
}