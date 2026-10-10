using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class Conversation : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string Title { get; set; } = "New conversation";
    public bool IsPinned { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Guid? LastModelId { get; set; }
    public AIModel? LastModel { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
