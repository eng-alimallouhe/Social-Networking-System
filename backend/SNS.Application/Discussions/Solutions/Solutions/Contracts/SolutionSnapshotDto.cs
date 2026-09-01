using SNS.Domain.Discussions.Solutions.Enums;

namespace SNS.Application.Discussions.Solutions.Solutions.Contracts;

/// <summary>
/// Represents a lightweight snapshot of a proposed solution.
/// </summary>
/// <param name="Id">The unique identifier of the solution.</param>
/// <param name="ProblemId">The associated problem identifier.</param>
/// <param name="Status">The solution status.</param>
/// <param name="CreatedAt">The timestamp when the solution was submitted.</param>
public sealed record SolutionSnapshotDto(
    Guid Id,
    Guid ProblemId,
    SolutionStatus Status,
    DateTime CreatedAt
);
