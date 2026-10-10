using System.Text;
using Chima.Application.Interfaces;

namespace Chima.Infrastructure.Extraction;

public class TxtExtractor : IDocumentExtractor
{
    private static readonly string[] Extensions = { ".txt", ".md", ".csv", ".json" };
    private static readonly string[] ContentTypes =
        { "text/plain", "text/markdown", "text/csv", "text/x-csv", "application/json", "text/json" };

    public bool CanHandle(string contentType, string fileName)
    {
        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        return Extensions.Contains(ext) || ContentTypes.Contains(contentType.ToLowerInvariant());
    }

    public async Task<ExtractedDocument> ExtractAsync(Stream content, string fileName, CancellationToken ct)
    {
        using var reader = new StreamReader(content, Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
        var text = await reader.ReadToEndAsync(ct);
        return new ExtractedDocument { Text = text };
    }
}