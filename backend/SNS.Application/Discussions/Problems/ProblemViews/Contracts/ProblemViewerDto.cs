namespace SNS.Application.Discussions.Problems.ProblemViews.Contracts;

/// <summary>
/// Represents profile details of a user who viewed a discussion problem.
/// </summary>
/// <param name="ProfileId">The unique identifier of the viewer profile.</param>
/// <param name="FullName">The full name of the viewer.</param>
/// <param name="Specialization">Optional professional specialization of the viewer.</param>
/// <param name="ProfilePictureUrl">Optional resolved public avatar URL.</param>
/// <param name="ViewedAt">The timestamp when the view occurred.</param>
public sealed record ProblemViewerDto(
    Guid ProfileId,
    string FullName,
    string? Specialization,
    string? ProfilePictureUrl,
    DateTime ViewedAt
);
