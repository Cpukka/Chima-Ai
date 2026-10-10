using Chima.Infrastructure.Persistence;
using Chima.Application.Common;
using Chima.Application.DTOs.Admin;
using Chima.Application.Interfaces;
using Chima.Application.Interfaces.Admin;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Services.Admin;

public class AdminSettingsService : IAdminSettingsService
{
    private readonly AppDbContext _db;
    private readonly IUnitOfWork _uow;

    public AdminSettingsService(AppDbContext db, IUnitOfWork uow) { _db = db; _uow = uow; }

    public async Task<IReadOnlyList<FeatureFlagDto>> ListFeatureFlagsAsync(CancellationToken ct)
    {
        var flags = await _db.FeatureFlags.OrderBy(f => f.Key).ToListAsync(ct);
        return flags.Select(f => new FeatureFlagDto
        {
            Id = f.Id,
            Key = f.Key,
            DisplayName = f.DisplayName,
            Description = f.Description,
            IsEnabled = f.IsEnabled,
            EnabledForPlansCsv = f.EnabledForPlansCsv,
            RolloutPercent = f.RolloutPercent,
            UpdatedAt = f.UpdatedAt
        }).ToList();
    }

    public async Task<FeatureFlagDto> UpdateFeatureFlagAsync(string key, UpdateFeatureFlagRequest request, CancellationToken ct)
    {
        var flag = await _db.FeatureFlags.FirstOrDefaultAsync(f => f.Key == key, ct)
            ?? throw new NotFoundException($"Feature flag '{key}' not found.");

        if (request.IsEnabled.HasValue) flag.IsEnabled = request.IsEnabled.Value;
        if (request.EnabledForPlansCsv is not null) flag.EnabledForPlansCsv = request.EnabledForPlansCsv;
        if (request.RolloutPercent.HasValue) flag.RolloutPercent = Math.Clamp(request.RolloutPercent.Value, 0, 100);

        await _uow.SaveChangesAsync(ct);
        return new FeatureFlagDto
        {
            Id = flag.Id,
            Key = flag.Key,
            DisplayName = flag.DisplayName,
            Description = flag.Description,
            IsEnabled = flag.IsEnabled,
            EnabledForPlansCsv = flag.EnabledForPlansCsv,
            RolloutPercent = flag.RolloutPercent,
            UpdatedAt = flag.UpdatedAt
        };
    }

    public async Task<IReadOnlyList<SystemSettingDto>> ListSystemSettingsAsync(CancellationToken ct)
    {
        var settings = await _db.SystemSettings.OrderBy(s => s.Key).ToListAsync(ct);
        return settings.Select(s => new SystemSettingDto
        {
            Id = s.Id,
            Key = s.Key,
            Value = s.IsSecret ? "Ã¢â‚¬Â¢Ã¢â‚¬Â¢Ã¢â‚¬Â¢Ã¢â‚¬Â¢Ã¢â‚¬Â¢Ã¢â‚¬Â¢" : s.Value,
            ValueType = s.ValueType,
            Description = s.Description,
            IsSecret = s.IsSecret,
            UpdatedAt = s.UpdatedAt
        }).ToList();
    }

    public async Task<SystemSettingDto> UpdateSystemSettingAsync(string key, UpdateSystemSettingRequest request, CancellationToken ct)
    {
        var setting = await _db.SystemSettings.FirstOrDefaultAsync(s => s.Key == key, ct)
            ?? throw new NotFoundException($"Setting '{key}' not found.");

        setting.Value = request.Value;
        await _uow.SaveChangesAsync(ct);

        return new SystemSettingDto
        {
            Id = setting.Id,
            Key = setting.Key,
            Value = setting.IsSecret ? "Ã¢â‚¬Â¢Ã¢â‚¬Â¢Ã¢â‚¬Â¢Ã¢â‚¬Â¢Ã¢â‚¬Â¢Ã¢â‚¬Â¢" : setting.Value,
            ValueType = setting.ValueType,
            Description = setting.Description,
            IsSecret = setting.IsSecret,
            UpdatedAt = setting.UpdatedAt
        };
    }
}