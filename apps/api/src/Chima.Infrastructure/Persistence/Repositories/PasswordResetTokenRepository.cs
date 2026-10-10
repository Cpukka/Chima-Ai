using Chima.Application.Interfaces;
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Persistence.Repositories;

public class PasswordResetTokenRepository : IPasswordResetTokenRepository
{
    private readonly AppDbContext _db;
    public PasswordResetTokenRepository(AppDbContext db) => _db = db;

    public Task<PasswordResetToken?> FindByHashAsync(string hash, CancellationToken ct) =>
        _db.PasswordResetTokens.Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == hash, ct);

    public Task<List<PasswordResetToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct) =>
        _db.PasswordResetTokens
            .Where(t => t.UserId == userId && t.ConsumedAt == null).ToListAsync(ct);

    public async Task AddAsync(PasswordResetToken token, CancellationToken ct) =>
        await _db.PasswordResetTokens.AddAsync(token, ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}