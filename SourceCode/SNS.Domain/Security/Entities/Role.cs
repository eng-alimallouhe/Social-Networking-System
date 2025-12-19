
using SNS.Domain.Abstractions.Common;

namespace SNS.Domain.Security;

public class Role : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    public required string RoleType { get; set; }

    // Soft Delete
    public bool IsActive { get; set; } = true;
}
