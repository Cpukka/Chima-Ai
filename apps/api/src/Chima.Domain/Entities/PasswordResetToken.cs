using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class PasswordResetToken : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime? ConsumedAt { get; set; }
    public string? RequestedFromIp { get; set; }

    public bool IsActive => ConsumedAt is null && DateTime.UtcNow < ExpiresAt;
}