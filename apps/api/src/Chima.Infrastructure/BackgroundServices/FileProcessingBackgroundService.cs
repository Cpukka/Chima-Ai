using Chima.Application.Interfaces;
using Chima.Domain.Enums;
using Chima.Infrastructure.Extraction;
using Chima.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Pgvector;

namespace Chima.Infrastructure.BackgroundServices;

public class FileProcessingBackgroundService : BackgroundService
{
    private static readonly TimeSpan Interval = TimeSpan.FromSeconds(10);
    private readonly IServiceProvider _sp;
    private readonly ILogger<FileProcessingBackgroundService> _logger;

    public FileProcessingBackgroundService(IServiceProvider sp, ILogger<FileProcessingBackgroundService> logger)
    {
        _sp = sp;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("File processing background service started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOnePendingAsync(stoppingToken);
            }
            catch (OperationCanceledException) { break; }
            catch (Exception ex) { _logger.LogError(ex, "File processing iteration failed."); }

            try { await Task.Delay(Interval, stoppingToken); }
            catch (OperationCanceledException) { break; }
        }

        _logger.LogInformation("File processing background service stopped.");
    }

    private async Task ProcessOnePendingAsync(CancellationToken ct)
    {
        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var storage = scope.ServiceProvider.GetRequiredService<IFileStorageProvider>();
        var extractors = scope.ServiceProvider.GetRequiredService<DocumentExtractorRegistry>();
        var chunker = scope.ServiceProvider.GetRequiredService<IChunkingService>();
        var embedder = scope.ServiceProvider.GetRequiredService<IEmbeddingService>();

        var file = await db.UploadedFiles
            .Where(f => f.ProcessingStatus == FileProcessingStatus.Pending
                     || f.ProcessingStatus == FileProcessingStatus.Extracting
                     || f.ProcessingStatus == FileProcessingStatus.Chunking
                     || f.ProcessingStatus == FileProcessingStatus.Embedding)
            .OrderBy(f => f.CreatedAt)
            .FirstOrDefaultAsync(ct);

        if (file is null) return;

        _logger.LogInformation("Processing file {FileId} '{Name}'", file.Id, file.OriginalFileName);

        try
        {
            // 1. Extract
            file.ProcessingStatus = FileProcessingStatus.Extracting;
            await db.SaveChangesAsync(ct);

            await using var stream = await storage.OpenAsync(file.StorageKey, ct);
            var extractor = extractors.Resolve(file.ContentType, file.OriginalFileName);
            var doc = await extractor.ExtractAsync(stream, file.OriginalFileName, ct);

            if (string.IsNullOrWhiteSpace(doc.Text))
                throw new InvalidOperationException("No text content could be extracted.");

            file.ExtractedCharCount = doc.Text.Length;
            file.PageCount = doc.PageCount;

            // 2. Chunk
            file.ProcessingStatus = FileProcessingStatus.Chunking;
            await db.SaveChangesAsync(ct);

            var chunks = doc.Pages is { Count: > 0 }
                ? doc.Pages.SelectMany(p => chunker.Chunk(p.Text, p.PageNumber)).ToList()
                : chunker.Chunk(doc.Text).ToList();

            if (chunks.Count == 0)
                throw new InvalidOperationException("Document produced no usable chunks.");

            // 3. Embed (best-effort — if the provider is unavailable, chunks are still stored)
            file.ProcessingStatus = FileProcessingStatus.Embedding;
            await db.SaveChangesAsync(ct);

            IReadOnlyList<float[]>? vectors = null;
            try
            {
                vectors = await embedder.EmbedBatchAsync(chunks.Select(c => c.Content).ToList(), ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Embedding failed for file {FileId}; storing chunks without vectors.", file.Id);
            }

            // 4. Persist chunks
            var entities = chunks.Select((c, i) => new Domain.Entities.FileChunk
            {
                FileId = file.Id,
                ChunkIndex = c.Index,
                Content = c.Content,
                TokenCount = c.TokenCount,
                PageNumber = c.PageNumber,
                Embedding = vectors is not null && i < vectors.Count ? new Vector(vectors[i]) : null,
                EmbeddingModel = vectors is not null ? "text-embedding-3-small" : null
            }).ToList();

            // Clear old chunks (idempotency)
            var old = await db.FileChunks.Where(c => c.FileId == file.Id).ToListAsync(ct);
            db.FileChunks.RemoveRange(old);

            db.FileChunks.AddRange(entities);
            file.ChunkCount = entities.Count;
            file.ProcessedAt = DateTime.UtcNow;
            file.ProcessingStatus = FileProcessingStatus.Ready;
            file.ProcessingError = null;
            await db.SaveChangesAsync(ct);

            _logger.LogInformation("File {FileId} processed: {Chunks} chunks, {Chars} chars.",
                file.Id, entities.Count, doc.Text.Length);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "File {FileId} processing failed.", file.Id);
            file.ProcessingStatus = FileProcessingStatus.Failed;
            file.ProcessingError = ex.Message;
            await db.SaveChangesAsync(CancellationToken.None);
        }
    }
}