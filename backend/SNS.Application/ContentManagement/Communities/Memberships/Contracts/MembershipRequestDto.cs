using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Domain.ContentManagement.Communities.Enums;

namespace SNS.Application.ContentManagement.Communities.Memberships.Contracts;

/// <summary>
/// Represents a pending or reviewed join request for a private community.
/// </summary>
/// <param name="RequestId">The unique identifier of the join request.</param>
/// <param name="CommunityId">The identifier of the community.</param>
/// <param name="Submitter">The profile snapshot of the requesting user.</param>
/// <param name="Status">The current status of the request.</param>
/// <param name="Notes">Optional message provided by the applicant.</param>
/// <param name="CreatedAt">The timestamp when the request was submitted.</param>
public sealed record MembershipRequestDto(
    Guid RequestId,
    Guid CommunityId,
    ProfileSnapshotDto Submitter,
    JoinRequestStatus Status,
    string Notes,
    DateTime CreatedAt
);
