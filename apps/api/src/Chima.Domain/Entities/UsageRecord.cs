using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class UsageRecord : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid? ConversationId { get; set; }
    public Guid? MessageId { get; set; }
    public Guid ModelId { get; set; }
    public AIModel Model { get; set; } = null!;

    public int InputTokens { get; set; }
    public int OutputTokens { get; set; }
    public int TotalTokens { get; set; }
    public decimal EstimatedCost { get; set; }
    public int DurationMs { get; set; }

    public string? RequestId { get; set; }
    public bool Success { get; set; }
    public string? ErrorCode { get; set; }
}
