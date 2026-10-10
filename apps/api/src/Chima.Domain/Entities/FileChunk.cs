using Chima.Domain.Common;
using Pgvector;

namespace Chima.Domain.Entities;

public class FileChunk : BaseEntity
{
    public Guid FileId { get; set; }
    public UploadedFile File { get; set; } = null!;
    public int ChunkIndex { get; set; }
    public string Content { get; set; } = string.Empty;
    public int TokenCount { get; set; }
    public int? PageNumber { get; set; }

    public Vector? Embedding { get; set; }
    public string? EmbeddingModel { get; set; }
}
