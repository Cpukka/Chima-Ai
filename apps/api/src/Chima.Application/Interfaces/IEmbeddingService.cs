namespace Chima.Application.Interfaces;

public interface IEmbeddingService
{
    /// <summary>
    /// Generate embeddings for a batch of texts.
    /// Returns one vector per input text (1536 dimensions for text-embedding-3-small).
    /// Throws if the embedding provider is unavailable.
    /// </summary>
    Task<IReadOnlyList<float[]>> EmbedBatchAsync(IReadOnlyList<string> texts, CancellationToken ct);

    /// <summary>True if an embedding provider is available (has API key, etc.).</summary>
    bool IsAvailable { get; }
}