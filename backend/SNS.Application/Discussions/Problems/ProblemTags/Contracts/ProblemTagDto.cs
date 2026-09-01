namespace SNS.Application.Discussions.Problems.ProblemTags.Contracts;

/// <summary>
/// Represents a tag associated with a discussion problem.
/// </summary>
/// <param name="TagId">The unique identifier of the tag.</param>
/// <param name="Name">The display name of the tag.</param>
public sealed record ProblemTagDto(
    Guid TagId,
    string Name
);
