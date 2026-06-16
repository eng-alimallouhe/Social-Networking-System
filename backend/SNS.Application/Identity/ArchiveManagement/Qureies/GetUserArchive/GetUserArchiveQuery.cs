using SNS.Application.Identity.ArchiveManagement.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;

namespace SNS.Application.Identity.ArchiveManagement.Qureies.GetUserArchive;

public sealed record GetUserArchiveQuery(
    Guid TargetUserId,
    int CurrentPage = 1,
    int PageSize = 10) : IQuery<Paged<UserArchiveSummaryDto>>;
