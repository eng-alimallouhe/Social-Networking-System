using SNS.Application.Education.Shared.DTOs;
using SNS.Application.Profiles.Profiles.Contracts;

namespace SNS.Application.Profiles.Profiles.Queries.GetProfileById;

/// <summary>
/// Represents a data transfer object used to
/// provide a comprehensive view of a user's profile.
/// </summary>
/// <param name="Id">Gets the unique identifier of the profile.</param>
/// <param name="FullName">Gets the full name of the profile owner. This value is used as the primary display name on the profile page.</param>
/// <param name="Bio">Gets the biography text. Optional.</param>
/// <param name="ProfilePictureUrl">Gets the URL of the profile picture. Optional.</param>
/// <param name="CoverImageUrl">Gets the URL of the cover image. Optional.</param>
/// <param name="Specialization">Gets the primary specialization. Optional.</param>
/// <param name="FollowersCount">Gets the total number of followers. This value is used to indicate the user's audience size.</param>
/// <param name="FollowingCount">Gets the total number of profiles this user is following.</param>
/// <param name="ViewsCount">Gets the total number of times the profile has been viewed.</param>
/// <param name="ProjectsCount">Gets the total number of projects associated with the profile.</param>
/// <param name="ProjectContributorsCount">Gets the total number of contributors across all projects.</param>
/// <param name="SolutionsCount">Gets the total number of solutions published by the user.</param>
/// <param name="Skills">Gets the list of professional skills.</param>
/// <param name="City">Gets the city of residence.</param>
/// <param name="University">Gets the associated university details. Optional.</param>
/// <param name="Faculty">Gets the associated faculty details. Optional.</param>
/// <param name="GitHubUrl">Gets the GitHub profile URL.</param>
/// <param name="LinkedInUrl">Gets the LinkedIn profile URL.</param>
/// <param name="XUrl">Gets the X (formerly Twitter) profile URL.</param>
/// <param name="FacebookUrl">Gets the Facebook profile URL.</param>
/// <param name="Website">Gets the personal website URL.</param>
/// <param name="IsFollowedByViewer">Indicates whether the current viewer is following this profile.</param>
/// <param name="IsBlocked">Indicates whether the profile is blocked by the viewer.</param>
/// <param name="IsBlockingViewer">Indicates whether the profile owner has blocked the viewer.</param>
/// <param name="ProfileViews">Gets the detailed profile view counter.</param>
public sealed record ProfileDetailsDto(
    Guid Id,
    string FullName,
    string? Bio,    
    string? ProfilePictureUrl,
    string? Specialization,
    int FollowersCount,
    int FollowingsCount,
    int ViewsCount,
    int ProjectsCount,
    int ProjectContributorsCount,
    int SolutionsCount,
    int ProblemsCount,
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
