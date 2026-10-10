using Chima.Application.DTOs.Admin;
using Chima.Application.Interfaces.Admin;
using Chima.Domain.Entities;

namespace Chima.Application.Interfaces.Admin;

public interface IAdminUserRepository
{
    Task<PagedAdminResult<AdminUserListItemDto>> ListAsync(
        int page, int pageSize, string? search, string? status, string? planCode, CancellationToken ct);
    Task<AdminUserDetailDto> GetDetailAsync(Guid userId, CancellationToken ct);
    Task<User?> GetUserAsync(Guid userId, CancellationToken ct);
}