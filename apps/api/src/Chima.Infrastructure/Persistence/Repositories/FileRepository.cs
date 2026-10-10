using Chima.Application.Interfaces;
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Pgvector;
using Pgvector.EntityFrameworkCore;

namespace Chima.Infrastructure.Persistence.Repositories;

public class FileRepository : IFileRepository
{
    private readonly AppDbContext _db;
    public FileRepository(AppDbContext db) => _db = db;

    public Task<UploadedFile?> GetOwnedAsync(Guid id, Guid userId, CancellationToken ct) =>
        _db.UploadedFiles.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId, ct);

    public Task<List<UploadedFile>> ListByUserAsync(Guid userId, CancellationToken ct) =>
        _db.UploadedFiles.Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(ct);

    public Task<List<UploadedFile>> ListByIdsOwnedAsync(IEnumerable<Guid> ids, Guid userId, CancellationToken ct)
    {
        var idList = ids.ToList();
        return _db.UploadedFiles.Where(f => f.UserId == userId && idList.Contains(f.Id)).ToListAsync(ct);
    }

    public async Task AddAsync(UploadedFile file, CancellationToken ct) =>
        await _db.UploadedFiles.AddAsync(file, ct);

    public Task DeleteAsync(UploadedFile file, CancellationToken ct)
    {
        _db.UploadedFiles.Remove(file);
        return Task.CompletedTask;
    }

    public Task<List<FileChunk>> GetChunksAsync(Guid fileId, CancellationToken ct) =>
        _db.FileChunks.Where(c => c.FileId == fileId).OrderBy(c => c.ChunkIndex).ToListAsync(ct);

    public async Task AddChunksAsync(IEnumerable<FileChunk> chunks, CancellationToken ct) =>
        await _db.FileChunks.AddRangeAsync(chunks, ct);

    public async Task DeleteChunksAsync(Guid fileId, CancellationToken ct)
    {
        var chunks = await _db.FileChunks.Where(c => c.FileId == fileId).ToListAsync(ct);
        _db.FileChunks.RemoveRange(chunks);
    }

    public async Task<List<(FileChunk Chunk, double Score)>> SearchChunksByVectorAsync(
        Guid userId, float[] queryVector, int topK, IEnumerable<Guid>? restrictToFileIds, CancellationToken ct)
    {
        var query = new Vector(queryVector);
        var q = _db.FileChunks
            .Include(c => c.File)
            .Where(c => c.File.UserId == userId && c.Embedding != null);

        if (restrictToFileIds is not null)
        {
            var ids = restrictToFileIds.ToList();
            q = q.Where(c => ids.Contains(c.FileId));
        }

        // Cosine distance: 1 - cosine_similarity
        var results = await q
            .OrderBy(c => c.Embedding!.CosineDistance(query))
            .Take(topK)
            .Select(c => new { Chunk = c, Distance = c.Embedding!.CosineDistance(query) })
            .ToListAsync(ct);

        return results.Select(r => (r.Chunk, 1.0 - r.Distance)).ToList();
    }

    public async Task<List<(FileChunk Chunk, double Score)>> SearchChunksByTextAsync(
        Guid userId, string query, int topK, IEnumerable<Guid>? restrictToFileIds, CancellationToken ct)
    {
        var like = $"%{query}%";

        var q = _db.FileChunks
            .Include(c => c.File)
            .Where(c => c.File.UserId == userId)
            .Where(c => EF.Functions.ILike(c.Content, like));

        if (restrictToFileIds is not null)
        {
            var ids = restrictToFileIds.ToList();
            q = q.Where(c => ids.Contains(c.FileId));
        }

        var results = await q.Take(topK).ToListAsync(ct);
        return results.Select(c => (c, 1.0)).ToList();
    }

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}