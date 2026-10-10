using Chima.Domain.Entities;

namespace Chima.Application.Interfaces;

public interface IFileRepository
{
    Task<UploadedFile?> GetOwnedAsync(Guid id, Guid userId, CancellationToken ct);
    Task<List<UploadedFile>> ListByUserAsync(Guid userId, CancellationToken ct);
    Task<List<UploadedFile>> ListByIdsOwnedAsync(IEnumerable<Guid> ids, Guid userId, CancellationToken ct);
    Task AddAsync(UploadedFile file, CancellationToken ct);
    Task DeleteAsync(UploadedFile file, CancellationToken ct);

    Task<List<FileChunk>> GetChunksAsync(Guid fileId, CancellationToken ct);
    Task AddChunksAsync(IEnumerable<FileChunk> chunks, CancellationToken ct);
    Task DeleteChunksAsync(Guid fileId, CancellationToken ct);

    /// <summary>Semantic search using pgvector cosine similarity. Requires embeddings.</summary>
    Task<List<(FileChunk Chunk, double Score)>> SearchChunksByVectorAsync(
        Guid userId, float[] queryVector, int topK, IEnumerable<Guid>? restrictToFileIds, CancellationToken ct);

    /// <summary>Fallback text search using ILIKE when embeddings are unavailable.</summary>
    Task<List<(FileChunk Chunk, double Score)>> SearchChunksByTextAsync(
        Guid userId, string query, int topK, IEnumerable<Guid>? restrictToFileIds, CancellationToken ct);

    Task SaveChangesAsync(CancellationToken ct);
}