using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Chima.Application.Services;

/// <summary>
/// Writes Server-Sent Events (SSE) frames to the HTTP response.
/// Format: "event: name\ndata: json\n\n"
/// </summary>
public static class SseWriter
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public static async Task WriteEventAsync(HttpResponse response, string eventName, object payload, CancellationToken ct)
    {
        var json = JsonSerializer.Serialize(payload, JsonOptions);
        var sb = new StringBuilder();
        sb.Append("event: ").Append(eventName).Append('\n');
        sb.Append("data: ").Append(json).Append('\n');
        sb.Append('\n');
        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        await response.Body.WriteAsync(bytes, ct);
        await response.Body.FlushAsync(ct);
    }

    public static void PrepareHeaders(HttpResponse response)
    {
        response.Headers["Content-Type"] = "text/event-stream";
        response.Headers["Cache-Control"] = "no-cache, no-store, no-transform";
        response.Headers["X-Accel-Buffering"] = "no";
        response.Headers["Connection"] = "keep-alive";
    }
}