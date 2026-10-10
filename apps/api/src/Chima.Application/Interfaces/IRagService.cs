using Chima.Domain.Entities;

namespace Chima.Application.Interfaces;

public interface IRagService
{
    /// <summary>
    /// Search document chunks semantically related to the query.
    /// Falls back to ILIKE text search if embeddings aren't available.
    /// </summary>
    Task<IReadOnlyList<RetrievedChunk>> SearchAsync(
        Guid userId,
        string query,
        int topK,
        IEnumerable<Guid>? restrictToFileIds,
        CancellationToken ct);

    /// <summary>Build a formatted context string from retrieved chunks for the AI prompt.</summary>
    string BuildContext(IReadOnlyList<RetrievedChunk> chunks);

    /// <summary>Format sources as structured references for the client.</summary>
    IReadOnlyList<SourceReference> BuildSources(IReadOnlyList<RetrievedChunk> chunks);
}

public class RetrievedChunk
{
    public Guid ChunkId { get; init; }
    public Guid FileId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public int ChunkIndex { get; init; }
    public int? PageNumber { get; init; }
    public string Content { get; init; } = string.Empty;
    public double Score { get; init; }
}

public class SourceReference
{
    public Guid FileId { get; init; }
    public string FileName { get; init; } = string.Empty;
    public int? PageNumber { get; init; }
    public int ChunkIndex { get; init; }
}