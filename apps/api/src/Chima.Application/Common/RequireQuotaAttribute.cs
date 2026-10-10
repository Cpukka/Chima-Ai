namespace Chima.Application.Common;

/// <summary>
/// Marker attribute for endpoints that consume a plan-metered quota.
/// Enforced by QuotaEnforcementFilter in the API layer.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public sealed class RequireQuotaAttribute : Attribute
{
    public string Feature { get; }
    public RequireQuotaAttribute(string feature) => Feature = feature;
}