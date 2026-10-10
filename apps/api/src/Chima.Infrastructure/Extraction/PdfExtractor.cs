using System.Text;
using Chima.Application.Interfaces;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace Chima.Infrastructure.Extraction;

public class PdfExtractor : IDocumentExtractor
{
    public bool CanHandle(string contentType, string fileName) =>
        fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase)
        || contentType.Equals("application/pdf", StringComparison.OrdinalIgnoreCase);

    public Task<ExtractedDocument> ExtractAsync(Stream content, string fileName, CancellationToken ct)
    {
        // PdfPig needs a seekable stream
        if (!content.CanSeek)
        {
            var ms = new MemoryStream();
            content.CopyTo(ms);
            ms.Position = 0;
            content = ms;
        }

        using var doc = PdfDocument.Open(content);
        var pages = new List<ExtractedPage>();
        var sb = new StringBuilder();

        foreach (var page in doc.GetPages())
        {
            var text = ExtractPageText(page);
            pages.Add(new ExtractedPage { PageNumber = page.Number, Text = text });
            sb.AppendLine($"[Page {page.Number}]");
            sb.AppendLine(text);
            sb.AppendLine();
        }

        return Task.FromResult(new ExtractedDocument
        {
            Text = sb.ToString().Trim(),
            PageCount = pages.Count,
            Pages = pages
        });
    }

    private static string ExtractPageText(Page page)
    {
        var sb = new StringBuilder();
        var words = page.GetWords().ToList();
        string? lastLine = null;

        foreach (var word in words)
        {
            var line = word.BoundingBox.Bottom;
            if (lastLine is not null && Math.Abs(line - double.Parse(lastLine)) > 2)
                sb.Append(' ');
            else if (sb.Length > 0 && sb[^1] != ' ')
                sb.Append(' ');

            sb.Append(word.Text);
            lastLine = line.ToString("F2");
        }

        return sb.ToString().Trim();
    }
}