using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Domain.Jobs.Enums;

namespace SNS.Application.Jobs.CompanyAdministrators.Contracts;

public sealed record CompanyAdministratorDto(
    Guid Id,
    Guid CompanyId,
    Guid ProfileId,
    ProfileSnapshotDto Profile,
    CompanyRole AdminRole
);
