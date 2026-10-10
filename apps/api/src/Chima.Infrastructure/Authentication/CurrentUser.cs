using System.Security.Claims;
using Chima.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Chima.Infrastructure.Authentication;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _http;

    public CurrentUser(IHttpContextAccessor http) => _http = http;

    public Guid? UserId
    {
        get
        {
            var sub = _http.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                   ?? _http.HttpContext?.User?.FindFirstValue("sub");
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }

    public string? Email => _http.HttpContext?.User?.FindFirstValue(ClaimTypes.Email)
                          ?? _http.HttpContext?.User?.FindFirstValue("email");

    public bool IsAuthenticated => _http.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public string? IpAddress => _http.HttpContext?.Connection?.RemoteIpAddress?.ToString();
}