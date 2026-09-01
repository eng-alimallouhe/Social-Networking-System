using SNS.Domain.Discussions.Solutions.Enums;

namespace SNS.Application.Discussions.Solutions.Solutions.Contracts;

/// <summary>
/// Represents a structured content block within a proposed solution.
/// </summary>
/// <param name="Id">The unique identifier of the content block.</param>
/// <param name="Type">The content block type (Text, Code, Image, Video).</param>
/// <param name="Content">The text or code content, or resolved media URL.</param>
/// <param name="ExtraInfo">Optional metadata such as programming language or caption.</param>
/// <param name="Order">The sequential display order index.</param>
public sealed record SolutionContentBlockDto(
    Guid Id,
    SolutionBlockType Type,
    string Content,
    string? ExtraInfo,
    int Order
);

/// <summary>
/// Payload contract for creating or updating a solution content block.
/// </summary>
/// <param name="Type">The content block type (Text, Code, Image, Video).</param>
/// <param name="Content">The textual content or media storage object key.</param>
/// <param name="ExtraInfo">Optional metadata such as code language.</param>
/// <param name="Order">The sequential order index.</param>
public sealed record CreateSolutionContentBlockDto(
    SolutionBlockType Type,
    string Content,
    string? ExtraInfo,
    int Order
);
