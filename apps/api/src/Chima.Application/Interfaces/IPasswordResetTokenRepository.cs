using Chima.Domain.Entities;

namespace Chima.Application.Interfaces;

public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken?> FindByHashAsync(string hash, CancellationToken ct);
    Task<List<PasswordResetToken>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct);
    Task AddAsync(PasswordResetToken token, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}