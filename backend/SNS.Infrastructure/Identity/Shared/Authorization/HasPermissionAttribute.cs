using Microsoft.AspNetCore.Authorization;

namespace SNS.Infrastructure.Identity.Shared.Authorization;

/// <summary>
/// Specifies that the class or method that this attribute is applied to requires the specified permission.
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
        : base(policy: permission)
    {
    }
}
