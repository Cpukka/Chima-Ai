using System.Text.Json;
using Chima.Application.Interfaces;
using Chima.Application.Interfaces.Admin;
using Chima.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Chima.Application.Services.Admin;

public class AuditLogService : IAuditLogService
{
    private readonly IAuditLogRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<AuditLogService> _logger;

    public AuditLogService(IAuditLogRepository repo, IUnitOfWork uow, ILogger<AuditLogService> logger)
    {
        _repo = repo; _uow = uow; _logger = logger;
    }

    public async Task LogAsync(
        Guid? actorUserId, string? actorEmail, string action,
        string? entityType = null, string? entityId = null,
        object? metadata = null, string? ipAddress = null, string? userAgent = null,
        CancellationToken ct = default)
    {
        try
        {
            var log = new AuditLog
            {
                ActorUserId = actorUserId,
                ActorEmail = actorEmail,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                MetadataJson = metadata is null ? null : JsonSerializer.Serialize(metadata),
                IpAddress = ipAddress,
                UserAgent = userAgent
            };
            await _repo.AddAsync(log, ct);
            await _uow.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to write audit log for action {Action}.", action);
        }
    }
}