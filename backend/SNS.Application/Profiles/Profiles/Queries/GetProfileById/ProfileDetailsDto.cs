using SNS.Application.Education.Shared.DTOs;
using SNS.Application.Profiles.Profiles.Contracts;

namespace SNS.Application.Profiles.Profiles.Queries.GetProfileById;

/// <summary>
/// Represents a data transfer object providing a detailed view of a user profile.
/// </summary>
/// <param name="Id">Gets the unique identifier of the profile.</param>
/// <param name="FullName">Gets the full display name of the profile owner.</param>
/// <param name="Bio">Gets the biography text. Optional.</param>
/// <param name="ProfilePictureUrl">Gets the public URL of the profile avatar image. Optional.</param>
/// <param name="Specialization">Gets the primary professional specialization. Optional.</param>
/// <param name="FollowersCount">Gets the total count of followers.</param>
/// <param name="FollowingsCount">Gets the total count of profiles followed by this user.</param>
/// <param name="ViewsCount">Gets the total profile view count.</param>
/// <param name="Skills">Gets the list of associated professional skills.</param>
/// <param name="AcademicRecordSummaryDtos">Gets the list of academic background record summaries.</param>
/// <param name="Location">Gets the location string. Optional.</param>
/// <param name="GitHubUrl">Gets the GitHub profile URL. Optional.</param>
/// <param name="LinkedInUrl">Gets the LinkedIn profile URL. Optional.</param>
/// <param name="XUrl">Gets the X (formerly Twitter) profile URL. Optional.</param>
/// <param name="FacebookUrl">Gets the Facebook profile URL. Optional.</param>
/// <param name="Website">Gets the personal website URL. Optional.</param>
/// <param name="IsFollowedByViewer">Indicates whether the current viewing user follows this profile.</param>
/// <param name="IsBlockedByViewer">Indicates whether the current viewing user has blocked this profile.</param>
/// <param name="IsViewerOwner">Indicates whether the current viewing user is the owner of this profile.</param>
/// <param name="IsBlockingViewer">Indicates whether this profile owner has blocked the current viewing user.</param>
public sealed record ProfileDetailsDto(
    Guid Id,
    string FullName,
    string? Bio,    
    string? ProfilePictureUrl,
    string? Specialization,
    int FollowersCount,
    int FollowingsCount,
    int ViewsCount,
    List<ProfileSkillDto> Skills,
    List<AcademicRecordSummaryDto> AcademicRecordSummaryDtos,
    string? Location,
    string? GitHubUrl,
    string? LinkedInUrl,
    string? XUrl,
    string? FacebookUrl,
    string? Website, 
    
    bool IsFollowedByViewer,
    
    bool IsBlockedByViewer,
    
    bool IsViewerOwner,

    bool IsBlockingViewer);