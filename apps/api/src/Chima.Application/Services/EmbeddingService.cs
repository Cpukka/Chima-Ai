using Chima.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Chima.Application.Services;

public class EmbeddingService : IEmbeddingService
{
    private const int MaxBatchSize = 20;
    private readonly IAIService _ai;
    private readonly ILogger<EmbeddingService> _logger;

    public EmbeddingService(IAIService ai, ILogger<EmbeddingService> logger)
    {
        _ai = ai;
        _logger = logger;
    }

    public bool IsAvailable => true; // verified at call time via provider failure

    public async Task<IReadOnlyList<float[]>> EmbedBatchAsync(IReadOnlyList<string> texts, CancellationToken ct)
    {
        var results = new List<float[]>(texts.Count);

        // Batch to avoid oversized requests
        for (int i = 0; i < texts.Count; i += MaxBatchSize)
        {
            var batch = texts.Skip(i).Take(MaxBatchSize).ToList();
            foreach (var text in batch)
            {
                var vector = await _ai.GenerateEmbeddingAsync(text, ct);
                results.Add(vector);
            }
            _logger.LogDebug("Embedded batch {Start}-{End} of {Total}", i, i + batch.Count, texts.Count);
        }

        return results;
    }
}