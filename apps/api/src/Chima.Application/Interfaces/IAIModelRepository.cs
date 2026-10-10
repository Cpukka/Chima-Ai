using Chima.Domain.Entities;

namespace Chima.Application.Interfaces;

public interface IAIModelRepository
{
    Task<AIModel?> GetByIdWithProviderAsync(Guid id, CancellationToken ct);
    Task<AIModel?> GetDefaultAsync(CancellationToken ct);
    Task<AIModel?> GetEmbeddingDefaultAsync(CancellationToken ct);
}