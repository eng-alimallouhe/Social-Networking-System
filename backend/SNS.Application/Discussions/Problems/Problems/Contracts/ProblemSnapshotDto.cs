using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Discussions.Shared.Enums;

namespace SNS.Application.Discussions.Problems.Problems.Contracts;

/// <summary>
/// Represents a lightweight snapshot of a discussion problem.
/// </summary>
/// <param name="Id">The unique identifier of the problem.</param>
/// <param name="Title">The problem title.</param>
/// <param name="Status">The problem status.</param>
/// <param name="Level">The difficulty level.</param>
/// <param name="CreatedAt">The timestamp when the problem was created.</param>
public sealed record ProblemSnapshotDto(
    Guid Id,
    string Title,
    ProblemStatus Status,
    DifficultyLevel Level,
    DateTime CreatedAt
);
