using Chima.Application.DTOs.Admin;

namespace Chima.Application.Interfaces.Admin;

public interface IAdminSettingsService
{
    Task<IReadOnlyList<FeatureFlagDto>> ListFeatureFlagsAsync(CancellationToken ct);
    Task<FeatureFlagDto> UpdateFeatureFlagAsync(string key, UpdateFeatureFlagRequest request, CancellationToken ct);

    Task<IReadOnlyList<SystemSettingDto>> ListSystemSettingsAsync(CancellationToken ct);
    Task<SystemSettingDto> UpdateSystemSettingAsync(string key, UpdateSystemSettingRequest request, CancellationToken ct);
}