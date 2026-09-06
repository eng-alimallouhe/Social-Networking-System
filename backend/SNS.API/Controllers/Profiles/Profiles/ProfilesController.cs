using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Contracts.Profiles;
using SNS.API.Extensions;
using SNS.API.Helpers;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Profiles.Profiles.Commands.CreateProfile;
using SNS.Application.Profiles.Profiles.Commands.UpdateBasicInformation;
using SNS.Application.Profiles.Profiles.Commands.UpdateProfilePicture;
using SNS.Application.Profiles.Profiles.Commands.UpdateSocialLinks;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Profiles.Profiles.Queries.GetBasicInformation;
using SNS.Application.Profiles.Profiles.Queries.GetProfileById;
using SNS.Application.Profiles.Profiles.Queries.GetProfileForCurrentUser;
using SNS.Application.Profiles.Profiles.Queries.GetProfilePictureUrl;
using SNS.Application.Profiles.Profiles.Queries.GetSocialLinks;
using SNS.Application.Shared.Contracts.Storage;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Profiles.Profiles;

/// <summary>
/// Handles user profile creation, profile details queries, picture updates, and social links management.
/// </summary>
[Route("api/v{version:apiVersion}/profiles/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves basic profile summary for the currently authenticated user.
    /// </summary>
    /// <response code="200">Returns current user's profile summary <see cref="ProfileBaseDto"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="404">Profile was not found for current user.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("base")]
    [ProducesResponseType(typeof(ProfileBaseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result<ProfileBaseDto>>> GetProfileForCurrentUserAsync()
    {
        return (await _mediator.Send(new GetProfileForCurrentUserQuery())).ToActionResult(this);
    }

    /// <summary>
    /// Creates a new user profile with initial details and optional profile picture.
    /// </summary>
    /// <param name="request">The multipart form request data for profile creation.</param>
    /// <response code="200">The profile was created successfully.</response>
    /// <response code="400">The profile creation parameters are invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="409">A profile already exists for the authenticated user.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Result<AuthTokenDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [RequireSession]
    public async Task<ActionResult<Result<AuthTokenDto>>> CreateProfileAsync([FromForm] CreateProfileRequest request)
    {
        UploadedFile? uploadedFile = null;

        if (request.ProfilePicture is not null)
        {
            uploadedFile = new UploadedFile(
                Stream: request.ProfilePicture.OpenReadStream(),
                FileName: request.ProfilePicture.FileName,
                ContentType: request.ProfilePicture.ContentType,
                Extension: Path.GetExtension(request.ProfilePicture.FileName)
                                .TrimStart('.')
                                .ToLowerInvariant(),
                Length: request.ProfilePicture.Length
            );
        }

        var result = await _mediator.Send(new CreateProfileCommand(
            FullName: request.FullName,
            Specialization: request.Specialization,
            Bio: request.Bio,
            ProfilePicture: uploadedFile
        ));

        if (result.Value != null && result.IsSuccess)
        {
            Response.Cookies.Append(
                CookieFactory.RefreshTokenCookieName,
                result.Value?.RefreshToken ?? string.Empty,
                CookieFactory.CreateRefreshTokenCookie(true));

            return (Result<AuthTokenDto>.Success(new AuthTokenDto(result.Value!.Token), result.StatusCode)).ToActionResult(this);
        }


        return (Result<AuthTokenDto>.Failure(result.StatusCode)).ToActionResult(this);
    }

    /// <summary>
    /// Updates basic information (full name, bio, specialization) of the authenticated user's profile.
    /// </summary>
    /// <param name="request">The basic information update payload.</param>
    /// <response code="200">Profile basic information updated successfully.</response>
    /// <response code="400">The update parameters are invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("basic-information")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result>> UpdateBasicInformationAsync([FromBody] UpdateBasicInformationCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Uploads and updates the profile avatar picture.
    /// </summary>
    /// <param name="profilePicture">The uploaded image file.</param>
    /// <response code="200">Profile picture updated successfully.</response>
    /// <response code="400">The uploaded file is empty or unsupported image format.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("profile-picture")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result>> UpdateProfilePictureAsync(IFormFile profilePicture)
    {
        return (await _mediator.Send(new UpdateProfilePictureCommand(profilePicture.ToUploadedFile()))).ToActionResult(this);
    }

    /// <summary>
    /// Updates social media links on the authenticated user's profile.
    /// </summary>
    /// <param name="request">The social links update payload.</param>
    /// <response code="200">Social links updated successfully.</response>
    /// <response code="400">One or more social URLs are invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("social-links")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result>> UpdateSocialLinksAsync([FromBody] UpdateSocialLinksCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves basic profile information for the authenticated user.
    /// </summary>
    /// <response code="200">Returns basic profile information <see cref="ProfileBaseDto"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("basic-information")]
    [ProducesResponseType(typeof(ProfileBaseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result<ProfileBaseDto>>> GetBasicInformationAsync()
    {
        return (await _mediator.Send(new GetBasicInformationQuery())).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves the public URL of the authenticated user's profile picture.
    /// </summary>
    /// <response code="200">Returns public profile picture URL string.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("profile-picture")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result<string>>> GetProfilePictureAsync()
    {
        return (await _mediator.Send(new GetProfilePictureUrlQuery())).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves social media links configured for the authenticated user's profile.
    /// </summary>
    /// <response code="200">Returns social media links DTO <see cref="SocialLinksDto"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("social-links")]
    [ProducesResponseType(typeof(SocialLinksDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result<SocialLinksDto>>> GetSocialLinksAsync()
    {
        return (await _mediator.Send(new GetSocialLinksQuery())).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves detailed profile information for a specific profile ID.
    /// </summary>
    /// <param name="id">The unique identifier of the target profile.</param>
    /// <response code="200">Returns comprehensive profile details <see cref="ProfileDetailsDto"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="404">The target profile was not found.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProfileDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result<ProfileDetailsDto>>> GetProfileAsync(Guid id)
    {
        return (await _mediator.Send(new GetProfileByIdQuery(id))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves candidate profiles eligible for a project contributor invitation.
    /// </summary>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("project-invitation-candidates")]
    [ProducesResponseType(typeof(List<ProfileInvitationCandidateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result<List<ProfileInvitationCandidateDto>>>> GetProfilesForProjectInvitationAsync(
        [FromQuery] Guid projectId,
        [FromQuery] string? search = null)
    {
        return (await _mediator.Send(new SNS.Application.Profiles.Profiles.Queries.GetProfilesForProjectInvitation.GetProfilesForProjectInvitationQuery(projectId, search))).ToActionResult(this);
    }
}

