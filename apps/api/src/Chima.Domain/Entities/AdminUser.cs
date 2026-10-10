using Chima.Domain.Common;
using Chima.Domain.Enums;

namespace Chima.Domain.Entities;

public class AdminUser : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public AdminRole Role { get; set; } = AdminRole.Support;
    public bool IsActive { get; set; } = true;
    public DateTime? LastActionAt { get; set; }
}
