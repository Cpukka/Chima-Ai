namespace Chima.Application.Interfaces;

public interface IUsageService
{
    Task RecordAsync(
        Guid userId,
        Guid? conversationId,
        Guid? messageId,
        Guid modelId,
        int inputTokens,
        int outputTokens,
        decimal estimatedCost,
        int durationMs,
        bool success,
        string? errorCode,
        string? requestId,
        CancellationToken ct);
}