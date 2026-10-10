using Chima.Application.Common;
using Chima.Application.DTOs.AI;
using Chima.Application.Interfaces;
using Chima.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chima.Infrastructure.AI;

/// <summary>
/// Thin alias provider for any OpenAI-compatible endpoint (Azure OpenAI, OpenRouter, Groq, etc.).
/// Currently delegates to OpenAIProvider logic through the same HTTP contract.
/// </summary>
public class CompatibleAIProvider : IAIProvider
{
    private readonly OpenAIProvider _inner;
    private readonly AISettings _settings;

    public CompatibleAIProvider(IOptions<AISettings> settings, IHttpClientFactory http, ILogger<OpenAIProvider> logger)
    {
        _settings = settings.Value;
        _inner = new OpenAIProvider(settings, http.CreateClient("OpenAI"), logger);
    }

    public string Name => "compatible";
    public bool SupportsModel(string modelIdentifier) => !string.IsNullOrWhiteSpace(modelIdentifier);
    public Task<AIChatResponse> ChatAsync(AIChatRequest request, CancellationToken ct) => _inner.ChatAsync(request, ct);
    public Task<string> GenerateTitleAsync(AITitleRequest request, CancellationToken ct) => _inner.GenerateTitleAsync(request, ct);
    public Task<float[]> GenerateEmbeddingAsync(string text, string modelIdentifier, CancellationToken ct) => _inner.GenerateEmbeddingAsync(text, modelIdentifier, ct);
    public Task<AIModerationResult> ModerateAsync(string text, CancellationToken ct) => _inner.ModerateAsync(text, ct);
}