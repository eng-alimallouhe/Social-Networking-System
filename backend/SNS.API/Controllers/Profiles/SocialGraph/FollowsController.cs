using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Contracts.Shared;
using SNS.API.Extensions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Profiles.SocialGraph.Commands.FollowProfile;
using SNS.Application.Profiles.SocialGraph.Commands.MuteProfile;
using SNS.Application.Profiles.SocialGraph.Commands.UnfollowProfile;
using SNS.Application.Profiles.SocialGraph.Commands.UnMuteProfile;
using SNS.Application.Profiles.SocialGraph.Contracts;
using SNS.Application.Profiles.SocialGraph.Queries.GetFollowSuggestions;
using SNS.Application.Profiles.SocialGraph.Queries.GetProfileFollowers;
using SNS.Application.Profiles.SocialGraph.Queries.GetProfileFollowings;
using SNS.Application.Shared.DTOs;
using SNS.Domain.Shared.Enums;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Profiles.SocialGraph;

/// <summary>
/// Handles social graph interactions including following, unfollowing, muting, querying followers/followings, and follow suggestions.
/// </summary>
[Route("api/v{version:apiVersion}/profiles/social-graph/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class FollowsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FollowsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves up to 10 follow suggestions for the authenticated profile based on mutual connections, shared skills, and popularity.
    /// </summary>
    /// <response code="200">Returns a list of profile suggestions <see cref="ProfileSummaryDto"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpGet("follow-suggestions")]
    [ProducesResponseType(typeof(List<ProfileSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<List<ProfileSummaryDto>>>> GetFollowSuggestionsAsync()
    {
        return (await _mediator.Send(new GetFollowSuggestionsQuery())).ToActionResult(this);
    }

    /// <summary>
    /// Establishes a follow relationship with a target profile.
    /// </summary>
    /// <param name="targetProfileId">The unique identifier of the profile to follow.</param>
    /// <response code="200">The profile was followed successfully.</response>
    /// <response code="400">Cannot follow own profile or already following.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="404">The target profile was not found.</response>
    [HttpPost("{targetProfileId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> FollowProfileAsync([FromRoute] Guid targetProfileId)
    {
        return (await _mediator.Send(new FollowProfileCommand(targetProfileId))).ToActionResult(this);
    }

    /// <summary>
    /// Removes an existing follow relationship with a target profile.
    /// </summary>
    /// <param name="targetProfileId">The unique identifier of the profile to unfollow.</param>
    /// <response code="200">The profile was unfollowed successfully.</response>
    /// <response code="400">No active follow relationship exists with the target profile.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="404">The target profile was not found.</response>
    [HttpDelete("{targetProfileId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> UnFollowProfileAsync([FromRoute] Guid targetProfileId)
    {
        return (await _mediator.Send(new UnfollowProfileCommand(targetProfileId))).ToActionResult(this);
    }

    /// <summary>
    /// Mutes notifications and feed updates from a target profile for a specified time period.
    /// </summary>
    /// <param name="targetProfileId">The unique identifier of the profile to mute.</param>
    /// <param name="period">The duration period for muting (e.g. EightHours, OneWeek, Forever).</param>
    /// <response code="200">The profile was muted successfully.</response>
    /// <response code="400">Invalid mute duration parameter.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="404">The target profile was not found.</response>
    [HttpPost("{targetProfileId:guid}/mute")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> MuteProfileAsync([FromRoute] Guid targetProfileId, [FromQuery] TimePeriod period)
    {
        return (await _mediator.Send(new MuteProfileCommand(targetProfileId, period))).ToActionResult(this);
    }

    /// <summary>
    /// Removes mute status from a target profile, restoring feed updates and notifications.
    /// </summary>
    /// <param name="targetProfileId">The unique identifier of the profile to unmute.</param>
    /// <response code="200">The profile was unmuted successfully.</response>
    /// <response code="400">The target profile is not muted.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="404">The target profile was not found.</response>
    [HttpDelete("{targetProfileId:guid}/mute")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> UnMuteProfileAsync([FromRoute] Guid targetProfileId)
    {
        return (await _mediator.Send(new UnMuteProfileCommand(targetProfileId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paged collection of follower profiles for a target profile.
    /// </summary>
    /// <param name="profileId">The target profile ID whose followers are queried.</param>
    /// <param name="query">The search term and pagination query parameters.</param>
    /// <response code="200">Returns paged list of follower profiles <see cref="ProfileFollowDto"/>.</response>
    /// <response code="404">The target profile was not found.</response>
    [HttpGet("{profileId:guid}/followers")]
    [ProducesResponseType(typeof(Paged<ProfileFollowDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<Paged<ProfileFollowDto>>>> GetProfileFollowersAsync([FromRoute] Guid profileId, [FromQuery] SearchQueryFilter query)
    {
        return (await _mediator.Send(new GetProfileFollowersQuery(
            ProfileId: profileId,
            SearchTerm: query.SearchTerm,
            CurrentPage: query.CurrentPage,
            PageSize: query.PageSize))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paged collection of profiles followed by a target profile.
    /// </summary>
    /// <param name="profileId">The target profile ID whose followings are queried.</param>
    /// <param name="query">The search term and pagination query parameters.</param>
    /// <response code="200">Returns paged list of followed profiles <see cref="ProfileFollowDto"/>.</response>
    /// <response code="404">The target profile was not found.</response>
    [HttpGet("{profileId:guid}/followings")]
    [ProducesResponseType(typeof(Paged<ProfileFollowDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<Paged<ProfileFollowDto>>>> GetProfileFollowingsAsync([FromRoute] Guid profileId, [FromQuery] SearchQueryFilter query)
    {
        return (await _mediator.Send(new GetProfileFollowingsQuery(
            ProfileId: profileId,
            SearchTerm: query.SearchTerm,
            CurrentPage: query.CurrentPage,
            PageSize: query.PageSize))).ToActionResult(this);
    }
}