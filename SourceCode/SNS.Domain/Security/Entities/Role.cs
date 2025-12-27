using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Security.Entities;

public class Role : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    public required string Type { get; set; }

    // Soft Delete
    public bool IsActive { get; set; } = true;

    public Role()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        IsActive = true;
    }
}