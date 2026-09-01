using SNS.Domain.Jobs.Enums;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Jobs.Entities;

public class CompanyAdministrator : Entity, IHardDeletable
{
    // Primary Key:
    public Guid Id { get; private set; }

    // Foreign Key: One(Profile) to Many(CompanyAdministrator)
    public Guid ProfileId { get; private set; }

    // Foreign Key: One(Company) to Many(CompanyAdministrator)
    public Guid CompanyId { get; private set; }

    public CompanyRole AdminRole { get; private set; } = CompanyRole.Owner;

    // Navigation Properties:
    public Company Company { get; private set; } = null!;
    public Profile Profile { get; private set; } = null!;

    private CompanyAdministrator()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static CompanyAdministrator Create(Guid companyId, Guid profileId, CompanyRole adminRole = CompanyRole.Manager)
    {
        return new CompanyAdministrator
        {
            CompanyId = companyId,
            ProfileId = profileId,
            AdminRole = adminRole
        };
    }

    public void ChangeRole(CompanyRole newRole)
    {
        AdminRole = newRole;
    }
}
