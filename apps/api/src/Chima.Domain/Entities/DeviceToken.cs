using Chima.Domain.Common;

namespace Chima.Domain.Entities;

public class DeviceToken : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public string Token { get; set; } = string.Empty;
    public string Platform { get; set; } = "android";
    public bool IsActive { get; set; } = true;
    public DateTime? LastUsedAt { get; set; }
}
