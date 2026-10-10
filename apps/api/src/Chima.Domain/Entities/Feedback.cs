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
