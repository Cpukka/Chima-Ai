namespace Chima.Application.Interfaces;

public interface IFileStorageProvider
{
    /// <summary>Persist the file and return the storage key (opaque identifier).</summary>
    Task<string> SaveAsync(Stream content, string fileName, string contentType, CancellationToken ct);

    /// <summary>Open a readable stream for the given storage key.</summary>
    Task<Stream> OpenAsync(string storageKey, CancellationToken ct);

    /// <summary>Delete the file.</summary>
    Task DeleteAsync(string storageKey, CancellationToken ct);

    /// <summary>Get a URL for direct client download (may be signed/temporary).</summary>
    Task<string> GetUrlAsync(string storageKey, CancellationToken ct);
}