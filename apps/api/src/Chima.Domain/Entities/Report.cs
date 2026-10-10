using Chima.Domain.Common;
using Chima.Domain.Enums;

namespace Chima.Domain.Entities;

public class Report : BaseEntity
{
    public Guid ReporterUserId { get; set; }
    public Guid? MessageId { get; set; }
    public Guid? ConversationId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Details { get; set; }
    public ReportStatus Status { get; set; } = ReportStatus.Open;
    public string? Resolution { get; set; }
    public Guid? ResolvedByAdminId { get; set; }
    public DateTime? ResolvedAt { get; set; }
}
