
namespace SNS.Domain.Security;

public class Role
{
    // Primary Key
    public Guid Id { get; set; }

    public required string RoleType { get; set; }
    public bool IsActive { get; set; } = true;
}
