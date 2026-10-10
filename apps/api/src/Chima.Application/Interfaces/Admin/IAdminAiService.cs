using Chima.Application.DTOs.Admin;

namespace Chima.Application.Interfaces.Admin;

public interface IAdminAiService
{
    Task<IReadOnlyList<AdminAiProviderDto>> ListProvidersAsync(CancellationToken ct);
    Task<AdminAiProviderDto> CreateProviderAsync(CreateAiProviderRequest request, CancellationToken ct);
    Task<AdminAiProviderDto> UpdateProviderAsync(Guid id, UpdateAiProviderRequest request, CancellationToken ct);

    Task<IReadOnlyList<AdminAiModelDto>> ListModelsAsync(CancellationToken ct);
    Task<AdminAiModelDto> CreateModelAsync(CreateAiModelRequest request, CancellationToken ct);
    Task<AdminAiModelDto> UpdateModelAsync(Guid id, UpdateAiModelRequest request, CancellationToken ct);
    Task SetDefaultModelAsync(Guid id, CancellationToken ct);
}