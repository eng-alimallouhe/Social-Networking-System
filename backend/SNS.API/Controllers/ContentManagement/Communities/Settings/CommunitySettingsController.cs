using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Contracts.ContentManagement.Communities;
using SNS.API.Extensions;
using SNS.Application.ContentManagement.Communities.Settings.Commands.UpdateCommunitySettings;
using SNS.Application.ContentManagement.Communities.Settings.Contracts;
using SNS.Application.ContentManagement.Communities.Settings.Queries.GetCommunitySettings;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.ContentManagement.Communities.Settings;

/// <summary>
/// Handles community configuration settings retrieval and modifications.
/// </summary>
[Route("api/v{version:apiVersion}/content-managment/communities/{communityId:guid}/settings")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class CommunitySettingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommunitySettingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves the current configuration settings for a community.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <response code="200">Returns community settings <see cref="CommunitySettingsDto"/>.</response>
    /// <response code="404">The community was not found.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Result<CommunitySettingsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<CommunitySettingsDto>>> GetSettingsAsync([FromRoute] Guid communityId)
    {
        return (await _mediator.Send(new GetCommunitySettingsQuery(communityId))).ToActionResult(this);
    }

    /// <summary>
    /// Updates configuration settings for a community.
    /// </summary>
    /// <param name="communityId">The unique identifier of the community.</param>
    /// <param name="request">The settings update payload.</param>
    /// <response code="200">Settings updated successfully.</response>
    /// <response code="401">User is unauthenticated.</response>
    /// <response code="403">User lacks owner or moderator permissions.</response>
    /// <response code="404">The community was not found.</response>
    [HttpPut]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> UpdateSettingsAsync(
        [FromRoute] Guid communityId,
        [FromBody] UpdateCommunitySettingsRequest request)
    {
        var command = new UpdateCommunitySettingsCommand(
            communityId,
            request.AllowPostWithoutApproval,
            request.AllowInvitationsByMembers,
            request.AllowComments,
            request.AllowMediaUpload);

        return (await _mediator.Send(command)).ToActionResult(this);
    }
}
