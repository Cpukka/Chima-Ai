using Chima.Application.Interfaces;
using Chima.Domain.Entities;

namespace Chima.Infrastructure.AI;

public class ModelResolver : IModelResolver
{
    private readonly IAIModelRepository _repo;
    public ModelResolver(IAIModelRepository repo) => _repo = repo;

    public async Task<AIModel?> ResolveAsync(Guid? preferredModelId, CancellationToken ct)
    {
        if (preferredModelId is { } id)
        {
            var m = await _repo.GetByIdWithProviderAsync(id, ct);
            if (m is not null) return m;
        }
        return await _repo.GetDefaultAsync(ct);
    }

    public Task<AIModel?> ResolveDefaultAsync(CancellationToken ct) => _repo.GetDefaultAsync(ct);
}