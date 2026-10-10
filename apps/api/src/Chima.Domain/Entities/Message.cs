using Chima.Domain.Common;
using Chima.Domain.Enums;

namespace Chima.Domain.Entities;

public class Message : BaseEntity
{
    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;

    public MessageRole Role { get; set; }
    public MessageStatus Status { get; set; } = MessageStatus.Completed;
    public string Content { get; set; } = string.Empty;

    public Guid? ParentMessageId { get; set; }
    public Message? ParentMessage { get; set; }
    public Guid? BranchRootId { get; set; }

    public Guid? ModelId { get; set; }
    public AIModel? Model { get; set; }
    public int? InputTokens { get; set; }
    public int? OutputTokens { get; set; }
    public decimal? EstimatedCost { get; set; }
    public int? DurationMs { get; set; }

    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }

    public ICollection<MessageAttachment> Attachments { get; set; } = new List<MessageAttachment>();
}
