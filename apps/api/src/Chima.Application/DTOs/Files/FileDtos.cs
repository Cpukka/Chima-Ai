namespace Chima.Application.DTOs.Files;

public class UploadedFileDto
{
    public Guid Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeBytes { get; set; }
    public string ProcessingStatus { get; set; } = string.Empty;
    public string? ProcessingError { get; set; }
    public int? PageCount { get; set; }
    public int? ChunkCount { get; set; }
    public int? ExtractedCharCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}