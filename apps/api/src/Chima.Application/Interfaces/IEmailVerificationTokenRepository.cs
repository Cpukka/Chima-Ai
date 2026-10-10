using Chima.Domain.Entities;

namespace Chima.Application.Interfaces;

public interface IEmailVerificationTokenRepository
{
    Task<EmailVerificationToken?> FindByHashAsync(string hash, CancellationToken ct);
    Task AddAsync(EmailVerificationToken token, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}