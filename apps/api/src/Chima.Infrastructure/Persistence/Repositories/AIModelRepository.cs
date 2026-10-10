using Chima.Application.Interfaces;
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Persistence.Repositories;

public class AIModelRepository : IAIModelRepository
{
    private readonly AppDbContext _db;
    public AIModelRepository(AppDbContext db) => _db = db;

    public Task<AIModel?> GetByIdWithProviderAsync(Guid id, CancellationToken ct) =>
        _db.AIModels.Include(m => m.Provider)
            .FirstOrDefaultAsync(m => m.Id == id && m.IsActive, ct);

    public Task<AIModel?> GetDefaultAsync(CancellationToken ct) =>
        _db.AIModels
            .Where(m => m.IsActive && m.IsDefault && m.CapabilitiesCsv.Contains("Text"))
            .FirstOrDefaultAsync(ct)
        ?? _db.AIModels.Where(m => m.IsActive && m.CapabilitiesCsv.Contains("Text"))
            .OrderBy(m => m.IsPremium).FirstOrDefaultAsync(ct);

    public Task<AIModel?> GetEmbeddingDefaultAsync(CancellationToken ct) =>
        _db.AIModels.Where(m => m.IsActive && m.CapabilitiesCsv.Contains("Embeddings"))
            .FirstOrDefaultAsync(ct);
}