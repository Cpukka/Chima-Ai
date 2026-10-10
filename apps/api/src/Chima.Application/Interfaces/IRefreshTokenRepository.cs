using Chima.Domain.Entities;

namespace Chima.Application.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> FindByHashAsync(string hash, CancellationToken ct);
    Task<List<RefreshToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct);
    Task AddAsync(RefreshToken token, CancellationToken ct);
    Task RevokeAllForUserAsync(Guid userId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}