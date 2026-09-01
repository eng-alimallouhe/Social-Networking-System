using SNS.Domain.Resumes.Enums;

namespace SNS.Application.Resumes.Resumes.Contracts;

/// <summary>
/// Represents a compact snapshot of a resume when referenced or embedded within other features.
/// </summary>
/// <param name="Id">The unique identifier of the resume.</param>
/// <param name="OwnerId">The profile identifier of the resume owner.</param>
/// <param name="Title">The title or designation of the resume.</param>
/// <param name="Template">The visual layout template.</param>
/// <param name="PersonalPictureUrl">The temporary presigned URL for the personal picture.</param>
public sealed record ResumeSnapshotDto(
    Guid Id,
    Guid OwnerId,
    string Title,
    Template Template,
    string? PersonalPictureUrl
);
