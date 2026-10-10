using Chima.Domain.Common;
using Chima.Domain.Enums;

namespace Chima.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    public Guid? SubscriptionId { get; set; }
    public UserSubscription? Subscription { get; set; }

    public PaymentProvider Provider { get; set; }
    public string? ExternalPaymentId { get; set; }
    public string? ExternalOrderId { get; set; }

    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

    public DateTime? PaidAt { get; set; }
    public DateTime? RefundedAt { get; set; }
    public string? ReceiptUrl { get; set; }
    public string? RawEventJson { get; set; }
}
