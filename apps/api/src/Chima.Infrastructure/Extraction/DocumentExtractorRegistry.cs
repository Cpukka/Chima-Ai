using Chima.Application.Common;
using Chima.Application.Interfaces;

namespace Chima.Infrastructure.Extraction;

public class DocumentExtractorRegistry
{
    private readonly IReadOnlyList<IDocumentExtractor> _extractors;

    public DocumentExtractorRegistry(IEnumerable<IDocumentExtractor> extractors) => _extractors = extractors.ToList();

    public IDocumentExtractor Resolve(string contentType, string fileName)
    {
        var extractor = _extractors.FirstOrDefault(e => e.CanHandle(contentType, fileName));
        return extractor ?? throw new ValidationException($"No extractor available for {fileName} ({contentType}).");
    }
}