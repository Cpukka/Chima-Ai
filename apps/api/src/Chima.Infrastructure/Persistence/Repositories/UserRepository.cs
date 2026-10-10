using Chima.Application.Interfaces;
using Chima.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public Task<User?> FindByEmailAsync(string email, CancellationToken ct) =>
        _db.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<User?> FindByIdAsync(Guid id, CancellationToken ct) =>
        _db.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<User?> FindByGoogleSubjectAsync(string subject, CancellationToken ct) =>
        _db.Users.Include(u => u.Profile).FirstOrDefaultAsync(u => u.GoogleSubjectId == subject, ct);

    public Task<bool> EmailExistsAsync(string email, CancellationToken ct) =>
        _db.Users.AnyAsync(u => u.Email == email, ct);

    public async Task AddAsync(User user, CancellationToken ct) => await _db.Users.AddAsync(user, ct);

    public Task SaveChangesAsync(CancellationToken ct) => _db.SaveChangesAsync(ct);
}