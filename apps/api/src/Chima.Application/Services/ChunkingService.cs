using Chima.Application.Interfaces;

namespace Chima.Application.Services;

public class ChunkingService : IChunkingService
{
    // ~800 tokens target. English averages ~4 chars/token.
    private const int TargetChars = 3200;
    private const int OverlapChars = 400;
    private const int MinChunkChars = 200;

    public IReadOnlyList<TextChunk> Chunk(string text, int? pageNumber = null)
    {
        if (string.IsNullOrWhiteSpace(text))
            return Array.Empty<TextChunk>();

        // Normalize whitespace
        text = text.Replace("\r\n", "\n").Replace("\r", "\n");

        var chunks = new List<TextChunk>();
        int index = 0;
        int pos = 0;

        while (pos < text.Length)
        {
            int remaining = text.Length - pos;
            int take = Math.Min(TargetChars, remaining);
            int end = pos + take;

            // Try to end at a paragraph break, sentence end, or space
            if (end < text.Length)
            {
                int candidate = text.LastIndexOf("\n\n", end - 1, Math.Min(500, take));
                if (candidate < pos + MinChunkChars)
                    candidate = text.LastIndexOf(". ", end - 1, Math.Min(500, take));
                if (candidate < pos + MinChunkChars)
                    candidate = text.LastIndexOf(' ', end - 1, Math.Min(500, take));
                if (candidate > pos + MinChunkChars)
                    end = candidate + 1;
            }

            var content = text.Substring(pos, end - pos).Trim();
            if (content.Length >= MinChunkChars || remaining < MinChunkChars)
            {
                chunks.Add(new TextChunk
                {
                    Index = index++,
                    Content = content,
                    TokenCount = EstimateTokens(content),
                    PageNumber = pageNumber
                });
            }

            // Advance with overlap
            if (end >= text.Length) break;
            pos = Math.Max(pos + 1, end - OverlapChars);
        }

        return chunks;
    }

    private static int EstimateTokens(string s) => Math.Max(1, s.Length / 4);
}