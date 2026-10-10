using Chima.Application.Interfaces;
using Chima.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Chima.Application.Services;

public class UsageService : IUsageService
{
    private readonly IUsageRecordRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<UsageService> _logger;

    public UsageService(IUsageRecordRepository repo, IUnitOfWork uow, ILogger<UsageService> logger)
    {
        _repo = repo; _uow = uow; _logger = logger;
    }

    public async Task RecordAsync(
        Guid userId, Guid? conversationId, Guid? messageId, Guid modelId,
        int inputTokens, int outputTokens, decimal estimatedCost, int durationMs,
        bool success, string? errorCode, string? requestId, CancellationToken ct)
    {
        try
        {
            await _repo.AddAsync(new UsageRecord
            {
                UserId = userId,
                ConversationId = conversationId,
                MessageId = messageId,
                ModelId = modelId,
                InputTokens = inputTokens,
                OutputTokens = outputTokens,
                TotalTokens = inputTokens + outputTokens,
                EstimatedCost = estimatedCost,
                DurationMs = durationMs,
                Success = success,
                ErrorCode = errorCode,
                RequestId = requestId
            }, ct);
            await _uow.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record usage for user {UserId}.", userId);
        }
    }
}