using Chima.Application.DTOs.Admin;

namespace Chima.Application.Interfaces.Admin;

public interface IAdminUserService
{
    Task<PagedAdminResult<AdminUserListItemDto>> ListAsync(
        int page, int pageSize, string? search, string? status, string? planCode, CancellationToken ct);
    Task<AdminUserDetailDto> GetAsync(Guid userId, CancellationToken ct);
    Task SuspendAsync(Guid userId, string reason, CancellationToken ct);
    Task UnsuspendAsync(Guid userId, CancellationToken ct);
    Task ChangePlanAsync(Guid userId, string planCode, string reason, CancellationToken ct);
}

public class PagedAdminResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int Total { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)Total / Math.Max(1, PageSize));
}