using Chima.Application.DTOs.AI;

namespace Chima.Application.Interfaces;

public interface IAIStreamProvider
{
    IAsyncEnumerable<AIStreamChunk> StreamAsync(AIChatRequest request, CancellationToken ct);
}