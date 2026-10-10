using Chima.Domain.Entities;

namespace Chima.Application.Interfaces;

public interface IModelResolver
{
    Task<AIModel?> ResolveAsync(Guid? preferredModelId, CancellationToken ct);
    Task<AIModel?> ResolveDefaultAsync(CancellationToken ct);
}