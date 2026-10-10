namespace Chima.Application.DTOs.AI;

public record AIChatMessage(string Role, string Content);

public class AIChatRequest
{
    public string Model { get; init; } = string.Empty;
    public IReadOnlyList<AIChatMessage> Messages { get; init; } = Array.Empty<AIChatMessage>();
    public double? Temperature { get; init; }
    public int? MaxTokens { get; init; }
    public string? SystemPrompt { get; init; }
    public CancellationToken CancellationToken { get; init; }
}

public class AIChatResponse
{
    public string Content { get; init; } = string.Empty;
    public int InputTokens { get; init; }
    public int OutputTokens { get; init; }
    public int TotalTokens => InputTokens + OutputTokens;
    public string ModelIdentifier { get; init; } = string.Empty;
    public string? FinishReason { get; init; }
    public int DurationMs { get; init; }
}

public class AITitleRequest
{
    public string FirstUserMessage { get; init; } = string.Empty;
    public string Model { get; init; } = string.Empty;
    public CancellationToken CancellationToken { get; init; }
}

public class AIModerationResult
{
    public bool Flagged { get; init; }
    public IReadOnlyList<string> Categories { get; init; } = Array.Empty<string>();
}