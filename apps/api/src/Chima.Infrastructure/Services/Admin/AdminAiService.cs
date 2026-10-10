using Chima.Infrastructure.Persistence;
using Chima.Application.Common;
using Chima.Application.DTOs.Admin;
using Chima.Application.Interfaces;
using Chima.Application.Interfaces.Admin;
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Services.Admin;

public class AdminAiService : IAdminAiService
{
    private readonly AppDbContext _db;
    private readonly IUnitOfWork _uow;

    public AdminAiService(AppDbContext db, IUnitOfWork uow)
    {
        _db = db; _uow = uow;
    }

    public async Task<IReadOnlyList<AdminAiProviderDto>> ListProvidersAsync(CancellationToken ct)
    {
        var providers = await _db.AIProviders.Include(p => p.Models).OrderBy(p => p.Name).ToListAsync(ct);
        return providers.Select(ToDto).ToList();
    }

    public async Task<AdminAiProviderDto> CreateProviderAsync(CreateAiProviderRequest request, CancellationToken ct)
    {
        var exists = await _db.AIProviders.AnyAsync(p => p.Name == request.Name, ct);
        if (exists) throw new ConflictException($"Provider '{request.Name}' already exists.");

        var provider = new AIProvider
        {
            Name = request.Name.Trim().ToLowerInvariant(),
            DisplayName = request.DisplayName.Trim(),
            BaseUrl = request.BaseUrl.Trim(),
            ApiKeySecretName = request.ApiKeySecretName.Trim(),
            IsActive = request.IsActive
        };
        _db.AIProviders.Add(provider);
        await _uow.SaveChangesAsync(ct);
        return ToDto(provider);
    }

    public async Task<AdminAiProviderDto> UpdateProviderAsync(Guid id, UpdateAiProviderRequest request, CancellationToken ct)
    {
        var provider = await _db.AIProviders.Include(p => p.Models).FirstOrDefaultAsync(p => p.Id == id, ct)
            ?? throw new NotFoundException("Provider not found.");

        if (request.DisplayName is not null) provider.DisplayName = request.DisplayName.Trim();
        if (request.BaseUrl is not null) provider.BaseUrl = request.BaseUrl.Trim();
        if (request.IsActive.HasValue) provider.IsActive = request.IsActive.Value;

        await _uow.SaveChangesAsync(ct);
        return ToDto(provider);
    }

    public async Task<IReadOnlyList<AdminAiModelDto>> ListModelsAsync(CancellationToken ct)
    {
        var models = await _db.AIModels.Include(m => m.Provider).OrderBy(m => m.Name).ToListAsync(ct);
        return models.Select(ToDto).ToList();
    }

    public async Task<AdminAiModelDto> CreateModelAsync(CreateAiModelRequest request, CancellationToken ct)
    {
        var exists = await _db.AIModels.AnyAsync(m => m.Name == request.Name, ct);
        if (exists) throw new ConflictException($"Model '{request.Name}' already exists.");

        var provider = await _db.AIProviders.FirstOrDefaultAsync(p => p.Id == request.ProviderId, ct)
            ?? throw new NotFoundException("Provider not found.");

        var model = new AIModel
        {
            ProviderId = request.ProviderId,
            Name = request.Name.Trim(),
            DisplayName = request.DisplayName.Trim(),
            ModelIdentifier = request.ModelIdentifier.Trim(),
            InputPricePer1M = request.InputPricePer1M,
            OutputPricePer1M = request.OutputPricePer1M,
            ContextWindow = request.ContextWindow,
            MaxOutputTokens = request.MaxOutputTokens,
            IsActive = request.IsActive,
            IsPremium = request.IsPremium,
            CapabilitiesCsv = request.CapabilitiesCsv
        };
        _db.AIModels.Add(model);
        await _uow.SaveChangesAsync(ct);

        model.Provider = provider;
        return ToDto(model);
    }

    public async Task<AdminAiModelDto> UpdateModelAsync(Guid id, UpdateAiModelRequest request, CancellationToken ct)
    {
        var model = await _db.AIModels.Include(m => m.Provider).FirstOrDefaultAsync(m => m.Id == id, ct)
            ?? throw new NotFoundException("Model not found.");

        if (request.DisplayName is not null) model.DisplayName = request.DisplayName.Trim();
        if (request.InputPricePer1M.HasValue) model.InputPricePer1M = request.InputPricePer1M.Value;
        if (request.OutputPricePer1M.HasValue) model.OutputPricePer1M = request.OutputPricePer1M.Value;
        if (request.ContextWindow.HasValue) model.ContextWindow = request.ContextWindow.Value;
        if (request.MaxOutputTokens.HasValue) model.MaxOutputTokens = request.MaxOutputTokens.Value;
        if (request.IsActive.HasValue) model.IsActive = request.IsActive.Value;
        if (request.IsPremium.HasValue) model.IsPremium = request.IsPremium.Value;
        if (request.CapabilitiesCsv is not null) model.CapabilitiesCsv = request.CapabilitiesCsv;

        await _uow.SaveChangesAsync(ct);
        return ToDto(model);
    }

    public async Task SetDefaultModelAsync(Guid id, CancellationToken ct)
    {
        var model = await _db.AIModels.FirstOrDefaultAsync(m => m.Id == id, ct)
            ?? throw new NotFoundException("Model not found.");

        // Clear existing default
        var current = await _db.AIModels.Where(m => m.IsDefault).ToListAsync(ct);
        foreach (var m in current) m.IsDefault = false;

        model.IsDefault = true;
        await _uow.SaveChangesAsync(ct);
    }

    private static AdminAiProviderDto ToDto(AIProvider p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        DisplayName = p.DisplayName,
        BaseUrl = p.BaseUrl,
        IsActive = p.IsActive,
        ApiKeySecretName = p.ApiKeySecretName,
        ModelCount = p.Models?.Count ?? 0
    };

    private static AdminAiModelDto ToDto(AIModel m) => new()
    {
        Id = m.Id,
        ProviderId = m.ProviderId,
        ProviderName = m.Provider?.Name ?? "",
        Name = m.Name,
        DisplayName = m.DisplayName,
        ModelIdentifier = m.ModelIdentifier,
        InputPricePer1M = m.InputPricePer1M,
        OutputPricePer1M = m.OutputPricePer1M,
        ContextWindow = m.ContextWindow,
        MaxOutputTokens = m.MaxOutputTokens,
        IsActive = m.IsActive,
        IsDefault = m.IsDefault,
        IsPremium = m.IsPremium,
        CapabilitiesCsv = m.CapabilitiesCsv
    };
}