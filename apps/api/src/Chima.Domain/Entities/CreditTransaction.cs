using Chima.Domain.Common;
using Chima.Domain.Enums;

namespace Chima.Domain.Entities;

public class CreditTransaction : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public CreditTransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public string? Reason { get; set; }
    public string? ReferenceId { get; set; }
}
