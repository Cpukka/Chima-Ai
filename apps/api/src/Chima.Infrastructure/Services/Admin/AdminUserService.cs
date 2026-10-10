using Chima.Application.Common;
using Chima.Application.DTOs.Admin;
using Chima.Application.Interfaces;
using Chima.Application.Interfaces.Admin;
using Chima.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Chima.Infrastructure.Services.Admin;

public class AdminUserService : IAdminUserService
{
    private readonly IAdminUserRepository _repo;
    private readonly ISubscriptionService _subs;
    private readonly IUnitOfWork _uow;

    public AdminUserService(IAdminUserRepository repo, ISubscriptionService subs, IUnitOfWork uow)
    {
        _repo = repo; _subs = subs; _uow = uow;
    }

    public Task<PagedAdminResult<AdminUserListItemDto>> ListAsync(
        int page, int pageSize, string? search, string? status, string? planCode, CancellationToken ct)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        return _repo.ListAsync(page, pageSize, search, status, planCode, ct);
    }

    public Task<AdminUserDetailDto> GetAsync(Guid userId, CancellationToken ct) =>
        _repo.GetDetailAsync(userId, ct);

    public async Task SuspendAsync(Guid userId, string reason, CancellationToken ct)
    {
        var user = await _repo.GetUserAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");
        user.Status = UserStatus.Suspended;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task UnsuspendAsync(Guid userId, CancellationToken ct)
    {
        var user = await _repo.GetUserAsync(userId, ct)
            ?? throw new NotFoundException("User not found.");
        user.Status = UserStatus.Active;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task ChangePlanAsync(Guid userId, string planCode, string reason, CancellationToken ct)
    {
        await _subs.ChangePlanAsync(userId, planCode, "manual", null, null, ct);
    }
}