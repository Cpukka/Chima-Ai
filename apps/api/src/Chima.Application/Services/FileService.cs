using Chima.Application.Common;
using Chima.Application.DTOs.Files;
using Chima.Application.Interfaces;
using Chima.Domain.Entities;
using Chima.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Chima.Application.Services;

public class FileService : IFileService
{
    private const long MaxFileSizeBytes = 100 * 1024 * 1024; // 100 MB
    private readonly IFileStorageProvider _storage;
    private readonly IFileRepository _repo;
    private readonly IUsageRepository _usageRepo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<FileService> _logger;

    public FileService(
        IFileStorageProvider storage,
        IFileRepository repo,
        IUsageRepository usageRepo,
        IUnitOfWork uow,
        ILogger<FileService> logger)
    {
        _storage = storage;
        _repo = repo;
        _usageRepo = usageRepo;
        _uow = uow;
        _logger = logger;
    }

    public async Task<UploadedFileDto> UploadAsync(
        Guid userId, Stream content, string fileName, string contentType, long sizeBytes, CancellationToken ct)
    {
        if (sizeBytes <= 0) throw new ValidationException("File is empty.");
        if (sizeBytes > MaxFileSizeBytes) throw new ValidationException("Maximum file size is 100 MB.");

        var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
        var allowed = new[] { ".txt", ".md", ".csv", ".pdf", ".docx", ".json" };
        if (string.IsNullOrEmpty(ext) || !allowed.Contains(ext))
            throw new ValidationException($"Unsupported file type. Allowed: {string.Join(", ", allowed)}");

        // Save to storage
        var key = await _storage.SaveAsync(content, fileName, contentType, ct);

        var file = new UploadedFile
        {
            UserId = userId,
            OriginalFileName = fileName,
            ContentType = contentType,
            SizeBytes = sizeBytes,
            StorageKey = key,
            ProcessingStatus = FileProcessingStatus.Pending
        };

        await _repo.AddAsync(file, ct);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("File uploaded: {FileName} ({Size} bytes) by user {UserId}, id {FileId}",
            fileName, sizeBytes, userId, file.Id);

        return ToDto(file);
    }

    public async Task<IReadOnlyList<UploadedFileDto>> ListAsync(Guid userId, CancellationToken ct)
    {
        var files = await _repo.ListByUserAsync(userId, ct);
        return files.Select(ToDto).ToList();
    }

    public async Task<UploadedFileDto?> GetAsync(Guid userId, Guid fileId, CancellationToken ct)
    {
        var file = await _repo.GetOwnedAsync(fileId, userId, ct);
        return file is null ? null : ToDto(file);
    }

    public async Task DeleteAsync(Guid userId, Guid fileId, CancellationToken ct)
    {
        var file = await _repo.GetOwnedAsync(fileId, userId, ct)
            ?? throw new NotFoundException("File not found.");

        // Delete chunks first (FK cascade would handle it, but be explicit)
        await _repo.DeleteChunksAsync(file.Id, ct);

        try
        {
            await _storage.DeleteAsync(file.StorageKey, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete storage object {Key}, continuing with DB deletion.", file.StorageKey);
        }

        await _repo.DeleteAsync(file, ct);
        await _uow.SaveChangesAsync(ct);
    }

    private static UploadedFileDto ToDto(UploadedFile f) => new()
    {
        Id = f.Id,
        OriginalFileName = f.OriginalFileName,
        ContentType = f.ContentType,
        SizeBytes = f.SizeBytes,
        ProcessingStatus = f.ProcessingStatus.ToString().ToLowerInvariant(),
        ProcessingError = f.ProcessingError,
        PageCount = f.PageCount,
        ChunkCount = f.ChunkCount,
        ExtractedCharCount = f.ExtractedCharCount,
        CreatedAt = f.CreatedAt,
        ProcessedAt = f.ProcessedAt
    };
}