using Chima.Application.Interfaces;
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Persistence.Repositories;

public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
{
    private readonly AppDbContext _db;
    public EmailVerificationTokenRepository(AppDbContext db) => _db = db;

    public Task<EmailVerificationToken?> FindByHashAsync(string hash, CancellationToken ct) =>
        _db.EmailVerificationTokens.Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TokenHash == hash, ct);

    public async Task AddAsync(EmailVerificationToken token, CancellationToken ct) =>
        await _db.EmailVerificationTokens.AddAsync(token, ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}