using Chima.Application.DTOs.AI;
using Chima.Domain.Entities;

namespace Chima.Application.Services;

/// <summary>
/// Builds the message array sent to the model. Enforces strict role separation
/// (system / user / assistant). Prevents prompt injection by keeping the system
/// prompt server-side and never accepting it from the client.
/// </summary>
public static class ContextBuilder
{
    public const int MaxMessages = 30;      // sliding window of recent turns
    public const int MaxCharsPerMessage = 12_000;

    public static IReadOnlyList<AIChatMessage> Build(IReadOnlyList<Message> history)
    {
        // Take the most recent N messages, oldest first
        var slice = history
            .OrderBy(m => m.CreatedAt)
            .TakeLast(MaxMessages)
            .ToList();

        var result = new List<AIChatMessage>(slice.Count);
        foreach (var m in slice)
        {
            var role = m.Role switch
            {
                Domain.Enums.MessageRole.User => "user",
                Domain.Enums.MessageRole.Assistant => "assistant",
                _ => "system"
            };

            var content = m.Content ?? string.Empty;
            if (content.Length > MaxCharsPerMessage)
                content = content[..MaxCharsPerMessage];

            result.Add(new AIChatMessage(role, content));
        }
        return result;
    }
}