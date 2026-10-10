using Chima.Application.DTOs.Files;

namespace Chima.Application.Interfaces;

public interface IFileService
{
    Task<UploadedFileDto> UploadAsync(
        Guid userId,
        Stream content,
        string fileName,
        string contentType,
        long sizeBytes,
        CancellationToken ct);

    Task<IReadOnlyList<UploadedFileDto>> ListAsync(Guid userId, CancellationToken ct);
    Task<UploadedFileDto?> GetAsync(Guid userId, Guid fileId, CancellationToken ct);
    Task DeleteAsync(Guid userId, Guid fileId, CancellationToken ct);
}