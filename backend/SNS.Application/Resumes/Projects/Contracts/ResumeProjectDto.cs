using SNS.Domain.Projects.Enums;

namespace SNS.Application.Resumes.Projects.Contracts;

/// <summary>
/// Represents a project linked to a resume.
/// </summary>
/// <param name="ResumeId">The identifier of the parent resume.</param>
/// <param name="ProjectId">The identifier of the linked project.</param>
/// <param name="Title">The title of the project.</param>
/// <param name="ShortDescription">A brief summary of the project.</param>
/// <param name="MainImageUrl">The resolved URL of the project's cover image.</param>
/// <param name="Type">The type classification of the project.</param>
/// <param name="Status">The lifecycle status of the project.</param>
public sealed record ResumeProjectDto(
    Guid ResumeId,
    Guid ProjectId,
    string Title,
    string ShortDescription,
    string? MainImageUrl,
    ProjectType Type,
    ProjectStatus Status
);
