namespace Chima.Application.Interfaces;

public interface IDocumentExtractor
{
    /// <summary>Whether this extractor can handle the given content type or file extension.</summary>
    bool CanHandle(string contentType, string fileName);

    /// <summary>
    /// Extract plain text from the stream.
    /// Returns the extracted text plus optional page metadata.
    /// </summary>
    Task<ExtractedDocument> ExtractAsync(Stream content, string fileName, CancellationToken ct);
}

public class ExtractedDocument
{
    public string Text { get; init; } = string.Empty;
    public int? PageCount { get; init; }
    public IReadOnlyList<ExtractedPage>? Pages { get; init; }
}

public class ExtractedPage
{
    public int PageNumber { get; init; }
    public string Text { get; init; } = string.Empty;
}