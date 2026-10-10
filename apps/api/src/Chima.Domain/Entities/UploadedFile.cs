using Chima.Domain.Common;
using Chima.Domain.Enums;

namespace Chima.Domain.Entities;

public class UploadedFile : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string StorageKey { get; set; } = string.Empty;
    public string? Sha256 { get; set; }

    public FileProcessingStatus ProcessingStatus { get; set; } = FileProcessingStatus.Pending;
    public string? ProcessingError { get; set; }
    public DateTime? ProcessedAt { get; set; }

    public int? ExtractedCharCount { get; set; }
    public int? ChunkCount { get; set; }
    public int? PageCount { get; set; }

    public ICollection<FileChunk> Chunks { get; set; } = new List<FileChunk>();
}
