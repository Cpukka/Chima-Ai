namespace Chima.Application.Interfaces;

public interface IChunkingService
{
    /// <summary>
    /// Split text into overlapping chunks suitable for embedding.
    /// Target: 800 tokens (~3200 chars) with 100-token overlap (~400 chars).
    /// </summary>
    IReadOnlyList<TextChunk> Chunk(string text, int? pageNumber = null);
}

public class TextChunk
{
    public int Index { get; init; }
    public string Content { get; init; } = string.Empty;
    public int TokenCount { get; init; }
    public int? PageNumber { get; init; }
}