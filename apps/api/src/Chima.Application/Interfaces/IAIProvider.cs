using Chima.Application.DTOs.AI;

namespace Chima.Application.Interfaces;

public interface IAIProvider
{
    /// <summary>Provider name as stored in ai_providers.name (e.g. "openai").</summary>
    string Name { get; }

    /// <summary>True if this provider can serve the given model identifier.</summary>
    bool SupportsModel(string modelIdentifier);

    /// <summary>Single-turn chat completion, non-streaming.</summary>
    Task<AIChatResponse> ChatAsync(AIChatRequest request, CancellationToken ct);

    /// <summary>Generate a short conversation title from the first user message.</summary>
    Task<string> GenerateTitleAsync(AITitleRequest request, CancellationToken ct);

    /// <summary>Generate an embedding vector for the given text.</summary>
    Task<float[]> GenerateEmbeddingAsync(string text, string modelIdentifier, CancellationToken ct);

    /// <summary>Optional content moderation (returns non-flagged by default).</summary>
    Task<AIModerationResult> ModerateAsync(string text, CancellationToken ct);
}