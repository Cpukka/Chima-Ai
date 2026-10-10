using System.Diagnostics;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Chima.Application.Common;
using Chima.Application.DTOs.AI;
using Chima.Application.Interfaces;
using Chima.Application.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chima.Infrastructure.AI;

public class OpenAIProvider : IAIProvider, IAIStreamProvider
{
    private readonly AISettings _settings;
    private readonly HttpClient _http;
    private readonly ILogger<OpenAIProvider> _logger;

    public OpenAIProvider(IOptions<AISettings> settings, HttpClient http, ILogger<OpenAIProvider> logger)
    {
        _settings = settings.Value;
        _http = http;
        _logger = logger;

        _http.Timeout = Timeout.InfiniteTimeSpan;
        if (!string.IsNullOrWhiteSpace(_settings.ApiKey))
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.ApiKey);
    }

    public string Name => "openai";
    public bool SupportsModel(string modelIdentifier) => !string.IsNullOrWhiteSpace(modelIdentifier);

    public async Task<AIChatResponse> ChatAsync(AIChatRequest request, CancellationToken ct)
    {
        var url = $"{_settings.BaseUrl.TrimEnd('/')}/chat/completions";
        var body = new
        {
            model = request.Model,
            messages = BuildMessages(request.SystemPrompt, request.Messages),
            temperature = request.Temperature ?? 0.7,
            max_tokens = request.MaxTokens
        };

        var sw = Stopwatch.StartNew();
        var payload = JsonSerializer.Serialize(body);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(_settings.TimeoutSeconds));

        var resp = await SendWithRetryAsync(url, payload, cts.Token);
        sw.Stop();

        var json = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
        {
            _logger.LogError("OpenAI error {Status}: {Body}", (int)resp.StatusCode, json);
            throw new AppException("ai_provider_error", $"AI provider error: {resp.StatusCode}", 502);
        }

        var parsed = JsonSerializer.Deserialize<OpenAIChatResponse>(json)
                     ?? throw new AppException("ai_provider_error", "Empty response from AI provider.", 502);

        var choice = parsed.Choices?.FirstOrDefault()
                     ?? throw new AppException("ai_provider_error", "No choices in AI response.", 502);

        return new AIChatResponse
        {
            Content = choice.Message?.Content ?? string.Empty,
            InputTokens = parsed.Usage?.PromptTokens ?? 0,
            OutputTokens = parsed.Usage?.CompletionTokens ?? 0,
            ModelIdentifier = parsed.Model ?? request.Model,
            FinishReason = choice.FinishReason,
            DurationMs = (int)sw.ElapsedMilliseconds
        };
    }

    public async Task<string> GenerateTitleAsync(AITitleRequest request, CancellationToken ct)
    {
        var url = $"{_settings.BaseUrl.TrimEnd('/')}/chat/completions";
        var body = new
        {
            model = request.Model,
            messages = new object[]
            {
                new { role = "system", content = "Generate a concise 3-to-6 word title. Return only the title, no quotes, no period." },
                new { role = "user", content = request.FirstUserMessage }
            },
            temperature = 0.4,
            max_tokens = 32
        };

        var payload = JsonSerializer.Serialize(body);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(_settings.TimeoutSeconds));

        var resp = await SendWithRetryAsync(url, payload, cts.Token);
        var json = await resp.Content.ReadAsStringAsync(ct);

        if (!resp.IsSuccessStatusCode)
        {
            _logger.LogWarning("Title generation failed {Status}: {Body}", (int)resp.StatusCode, json);
            return string.Empty;
        }

        var parsed = JsonSerializer.Deserialize<OpenAIChatResponse>(json);
        return parsed?.Choices?.FirstOrDefault()?.Message?.Content?.Trim() ?? string.Empty;
    }

    public async Task<float[]> GenerateEmbeddingAsync(string text, string modelIdentifier, CancellationToken ct)
    {
        var url = $"{_settings.BaseUrl.TrimEnd('/')}/embeddings";
        var body = new { model = modelIdentifier, input = text };
        var payload = JsonSerializer.Serialize(body);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(_settings.TimeoutSeconds));

        var resp = await SendWithRetryAsync(url, payload, cts.Token);
        var json = await resp.Content.ReadAsStringAsync(ct);
        if (!resp.IsSuccessStatusCode)
            throw new AppException("ai_provider_error", $"Embedding failed: {resp.StatusCode}", 502);

        using var doc = JsonDocument.Parse(json);
        var arr = doc.RootElement.GetProperty("data")[0].GetProperty("embedding");
        var result = new float[arr.GetArrayLength()];
        int i = 0;
        foreach (var el in arr.EnumerateArray()) result[i++] = el.GetSingle();
        return result;
    }

    public Task<AIModerationResult> ModerateAsync(string text, CancellationToken ct) =>
        Task.FromResult(new AIModerationResult());

    public async IAsyncEnumerable<AIStreamChunk> StreamAsync(
        AIChatRequest request,
        [EnumeratorCancellation] CancellationToken ct)
    {
        var url = $"{_settings.BaseUrl.TrimEnd('/')}/chat/completions";
        var body = new
        {
            model = request.Model,
            messages = BuildMessages(request.SystemPrompt, request.Messages),
            temperature = request.Temperature ?? 0.7,
            max_tokens = request.MaxTokens,
            stream = true,
            stream_options = new { include_usage = true }
        };

        var payload = JsonSerializer.Serialize(body);
        using var httpReq = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = new StringContent(payload, Encoding.UTF8, "application/json")
        };

        using var httpResp = await _http.SendAsync(httpReq, HttpCompletionOption.ResponseHeadersRead, ct);
        if (!httpResp.IsSuccessStatusCode)
        {
            var err = await httpResp.Content.ReadAsStringAsync(ct);
            _logger.LogError("OpenAI stream error {Status}: {Body}", (int)httpResp.StatusCode, err);
            throw new AppException("ai_provider_error", $"AI provider error: {httpResp.StatusCode}", 502);
        }

        await using var stream = await httpResp.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream, Encoding.UTF8);

        while (!reader.EndOfStream)
        {
            ct.ThrowIfCancellationRequested();

            var line = await reader.ReadLineAsync(ct);
            if (string.IsNullOrEmpty(line)) continue;
            if (!line.StartsWith("data: ")) continue;

            var data = line.Substring(6).Trim();
            if (data == "[DONE]") break;

            OpenAIStreamChunk? parsed;
            try { parsed = JsonSerializer.Deserialize<OpenAIStreamChunk>(data); }
            catch (JsonException) { continue; }

            if (parsed is null) continue;

            var delta = parsed.Choices?.FirstOrDefault()?.Delta?.Content;
            var finish = parsed.Choices?.FirstOrDefault()?.FinishReason;

            var isFinal = finish is not null;
            var inTok = parsed.Usage?.PromptTokens;
            var outTok = parsed.Usage?.CompletionTokens;

            if (!string.IsNullOrEmpty(delta) || isFinal || inTok.HasValue || outTok.HasValue)
            {
                yield return new AIStreamChunk
                {
                    Content = delta ?? string.Empty,
                    IsFinal = isFinal,
                    InputTokens = inTok,
                    OutputTokens = outTok,
                    FinishReason = finish
                };
            }
        }
    }

    private async Task<HttpResponseMessage> SendWithRetryAsync(string url, string payload, CancellationToken ct)
    {
        Exception? last = null;
        for (int attempt = 0; attempt <= _settings.MaxRetries; attempt++)
        {
            try
            {
                using var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var resp = await _http.PostAsync(url, content, ct);
                if ((int)resp.StatusCode < 500) return resp;
                last = new AppException("ai_provider_error", $"Server error {resp.StatusCode}", 502);
            }
            catch (TaskCanceledException) when (!ct.IsCancellationRequested)
            {
                last = new AppException("ai_timeout", "AI request timed out.", 504);
            }
            catch (HttpRequestException ex)
            {
                last = ex;
            }

            if (attempt < _settings.MaxRetries)
                await Task.Delay(TimeSpan.FromMilliseconds(300 * Math.Pow(2, attempt)), ct);
        }
        throw last ?? new AppException("ai_provider_error", "AI request failed.", 502);
    }

    private static List<object> BuildMessages(string? systemPrompt, IReadOnlyList<AIChatMessage> messages)
    {
        var list = new List<object>();
        if (!string.IsNullOrWhiteSpace(systemPrompt))
            list.Add(new { role = "system", content = systemPrompt });
        foreach (var m in messages)
            list.Add(new { role = m.Role, content = m.Content });
        return list;
    }

    private class OpenAIChatResponse
    {
        [JsonPropertyName("model")] public string? Model { get; set; }
        [JsonPropertyName("choices")] public List<Choice>? Choices { get; set; }
        [JsonPropertyName("usage")] public UsageInfo? Usage { get; set; }

        public class Choice
        {
            [JsonPropertyName("message")] public Msg? Message { get; set; }
            [JsonPropertyName("finish_reason")] public string? FinishReason { get; set; }
        }
        public class Msg
        {
            [JsonPropertyName("role")] public string? Role { get; set; }
            [JsonPropertyName("content")] public string? Content { get; set; }
        }
        public class UsageInfo
        {
            [JsonPropertyName("prompt_tokens")] public int PromptTokens { get; set; }
            [JsonPropertyName("completion_tokens")] public int CompletionTokens { get; set; }
            [JsonPropertyName("total_tokens")] public int TotalTokens { get; set; }
        }
    }

    private class OpenAIStreamChunk
    {
        [JsonPropertyName("model")] public string? Model { get; set; }
        [JsonPropertyName("choices")] public List<StreamChoice>? Choices { get; set; }
        [JsonPropertyName("usage")] public UsageInfo? Usage { get; set; }

        public class StreamChoice
        {
            [JsonPropertyName("delta")] public Delta? Delta { get; set; }
            [JsonPropertyName("finish_reason")] public string? FinishReason { get; set; }
        }
        public class Delta
        {
            [JsonPropertyName("role")] public string? Role { get; set; }
            [JsonPropertyName("content")] public string? Content { get; set; }
        }
        public class UsageInfo
        {
            [JsonPropertyName("prompt_tokens")] public int PromptTokens { get; set; }
            [JsonPropertyName("completion_tokens")] public int CompletionTokens { get; set; }
            [JsonPropertyName("total_tokens")] public int TotalTokens { get; set; }
        }
    }
}