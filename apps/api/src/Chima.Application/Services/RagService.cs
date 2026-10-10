using System.Text;
using Chima.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chima.Application.Services;

public class RagService : IRagService
{
    private readonly IFileRepository _files;
    private readonly IEmbeddingService _embeddings;
    private readonly ILogger<RagService> _logger;

    public RagService(IFileRepository files, IEmbeddingService embeddings, ILogger<RagService> logger)
    {
        _files = files;
        _embeddings = embeddings;
        _logger = logger;
    }

    public async Task<IReadOnlyList<RetrievedChunk>> SearchAsync(
        Guid userId, string query, int topK, IEnumerable<Guid>? restrictToFileIds, CancellationToken ct)
    {
        // Try semantic search first
        try
        {
            var vec = await _embeddings.EmbedBatchAsync(new[] { query }, ct);
            if (vec.Count == 1 && vec[0].Length > 0)
            {
                var hits = await _files.SearchChunksByVectorAsync(userId, vec[0], topK, restrictToFileIds, ct);
                if (hits.Count > 0)
                {
                    return hits.Select(h => new RetrievedChunk
                    {
                        ChunkId = h.Chunk.Id,
                        FileId = h.Chunk.FileId,
                        FileName = h.Chunk.File.OriginalFileName,
                        ChunkIndex = h.Chunk.ChunkIndex,
                        PageNumber = h.Chunk.PageNumber,
                        Content = h.Chunk.Content,
                        Score = h.Score
                    }).ToList();
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Semantic search failed, falling back to text search.");
        }

        // Fallback: keyword search
        var textHits = await _files.SearchChunksByTextAsync(userId, query, topK, restrictToFileIds, ct);
        return textHits.Select(h => new RetrievedChunk
        {
            ChunkId = h.Chunk.Id,
            FileId = h.Chunk.FileId,
            FileName = h.Chunk.File.OriginalFileName,
            ChunkIndex = h.Chunk.ChunkIndex,
            PageNumber = h.Chunk.PageNumber,
            Content = h.Chunk.Content,
            Score = h.Score
        }).ToList();
    }

    public string BuildContext(IReadOnlyList<RetrievedChunk> chunks)
    {
        if (chunks.Count == 0) return string.Empty;

        var sb = new StringBuilder();
        sb.AppendLine("You have access to the following excerpts from the user's uploaded documents.");
        sb.AppendLine("Use them to answer the user's question. Cite sources as [N] where N matches the excerpt number.");
        sb.AppendLine("If the excerpts don't contain the answer, say so and answer from general knowledge.");
        sb.AppendLine();

        for (int i = 0; i < chunks.Count; i++)
        {
            var c = chunks[i];
            sb.AppendLine($"[{i + 1}] Source: {c.FileName}" + (c.PageNumber.HasValue ? $" (page {c.PageNumber})" : ""));
            sb.AppendLine("---");
            sb.AppendLine(c.Content);
            sb.AppendLine("---");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public IReadOnlyList<SourceReference> BuildSources(IReadOnlyList<RetrievedChunk> chunks)
    {
        return chunks.Select(c => new SourceReference
        {
            FileId = c.FileId,
            FileName = c.FileName,
            PageNumber = c.PageNumber,
            ChunkIndex = c.ChunkIndex
        }).ToList();
    }
}