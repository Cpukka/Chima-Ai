using Chima.Application.DTOs.AI;
using Chima.Domain.Entities;

namespace Chima.Application.Interfaces;

public interface IAIService
{
    Task<AIResult> CompleteForConversationAsync(
        Conversation conversation,
        IReadOnlyList<Message> history,
        Guid userId,
        Guid? preferredModelId,
        CancellationToken ct);

    IAsyncEnumerable<AIStreamChunk> StreamForConversationAsync(
        Conversation conversation,
        IReadOnlyList<Message> history,
        Guid userId,
        Guid? preferredModelId,
        AIStreamContext context,
        CancellationToken ct);

    Task<string> GenerateTitleAsync(string firstUserMessage, CancellationToken ct);

    Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken ct);
}

public class AIResult
{
    public string Content { get; init; } = string.Empty;
    public Guid ModelId { get; init; }
    public string ModelName { get; init; } = string.Empty;
    public int InputTokens { get; init; }
    public int OutputTokens { get; init; }
    public decimal EstimatedCost { get; init; }
    public int DurationMs { get; init; }
}

/// <summary>
/// Passed by the caller so streaming can hand back model metadata before the first chunk.
/// </summary>
public class AIStreamContext
{
    public Guid ModelId { get; set; }
    public string ModelName { get; set; } = string.Empty;
    public string ModelIdentifier { get; set; } = string.Empty;
    public decimal InputPricePer1M { get; set; }
    public decimal OutputPricePer1M { get; set; }
}