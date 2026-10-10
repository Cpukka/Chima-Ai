namespace Chima.Application.Interfaces.Admin;

public interface IAuditLogService
{
    Task LogAsync(
        Guid? actorUserId,
        string? actorEmail,
        string action,
        string? entityType = null,
        string? entityId = null,
        object? metadata = null,
        string? ipAddress = null,
        string? userAgent = null,
        CancellationToken ct = default);
}