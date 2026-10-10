using System.Text;
using Chima.Application.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Chima.Infrastructure.Extraction;

public class DocxExtractor : IDocumentExtractor
{
    public bool CanHandle(string contentType, string fileName) =>
        fileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase)
        || contentType.Contains("wordprocessingml.document", StringComparison.OrdinalIgnoreCase);

    public Task<ExtractedDocument> ExtractAsync(Stream content, string fileName, CancellationToken ct)
    {
        if (!content.CanSeek)
        {
            var ms = new MemoryStream();
            content.CopyTo(ms);
            ms.Position = 0;
            content = ms;
        }

        using var doc = WordprocessingDocument.Open(content, false);
        var body = doc.MainDocumentPart?.Document?.Body;

        if (body is null)
            return Task.FromResult(new ExtractedDocument { Text = string.Empty });

        var sb = new StringBuilder();
        foreach (var para in body.Descendants<Paragraph>())
        {
            var text = string.Concat(para.Descendants<Text>().Select(t => t.Text));
            if (!string.IsNullOrWhiteSpace(text))
                sb.AppendLine(text);
        }

        return Task.FromResult(new ExtractedDocument { Text = sb.ToString().Trim() });
    }
}