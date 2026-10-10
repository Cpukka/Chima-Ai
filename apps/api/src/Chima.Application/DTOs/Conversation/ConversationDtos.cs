namespace Chima.Application.DTOs.Conversation;

public class ConversationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsPinned { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? DeletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public int MessageCount { get; set; }
    public string? LastMessagePreview { get; set; }
    public DateTime? LastMessageAt { get; set; }
}

public class CreateConversationRequest
{
    public string? Title { get; set; }
}

public class UpdateConversationRequest
{
    public string? Title { get; set; }
    public bool? IsPinned { get; set; }
    public bool? IsArchived { get; set; }
}