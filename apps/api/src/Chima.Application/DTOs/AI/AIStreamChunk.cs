namespace Chima.Application.DTOs.AI;

public class AIStreamChunk
{
    public string Content { get; init; } = string.Empty;
    public bool IsFinal { get; init; }
    public int? InputTokens { get; init; }
    public int? OutputTokens { get; init; }
    public string? FinishReason { get; init; }
}