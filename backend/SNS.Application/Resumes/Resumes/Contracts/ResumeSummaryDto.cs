using SNS.Domain.Resumes.Enums;
using SNS.Domain.Shared.Enums;

namespace SNS.Application.Resumes.Resumes.Contracts;

/// <summary>
/// Represents a summary view of a resume for list displays.
/// </summary>
/// <param name="Id">The unique identifier of the resume.</param>
/// <param name="OwnerId">The profile identifier of the resume owner.</param>
/// <param name="PersonalPictureUrl">The temporary presigned URL for the personal picture.</param>
/// <param name="SyncProfilePicture">Indicates whether the resume synchronizes its picture with the profile avatar.</param>
/// <param name="Title">The title or professional designation on the resume.</param>
/// <param name="Template">The visual layout template of the resume.</param>
/// <param name="Summary">The professional summary or executive statement.</param>
/// <param name="Language">The supported language of the resume.</param>
/// <param name="CreatedAt">The timestamp when the resume was created.</param>
/// <param name="UpdatedAt">The timestamp when the resume was last updated.</param>
public sealed record ResumeSummaryDto(
    Guid Id,
    Guid OwnerId,
    string? PersonalPictureUrl,
    bool SyncProfilePicture,
    string Title,
    Template Template,
    string Summary,
    SupportedLanguage Language,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
