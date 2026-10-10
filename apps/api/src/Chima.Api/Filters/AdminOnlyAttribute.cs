using Microsoft.AspNetCore.Authorization;

namespace Chima.Api.Filters;

/// <summary>
/// Restricts access to users whose JWT carries the "role" claim with value
/// Admin, SuperAdmin, or Support.
/// </summary>
public class AdminOnlyAttribute : AuthorizeAttribute
{
    public AdminOnlyAttribute() => Roles = "Admin,SuperAdmin,Support,Analyst";
}