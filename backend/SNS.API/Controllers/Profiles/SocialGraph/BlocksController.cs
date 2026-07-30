using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.DTOs.Shared;
using SNS.API.Extensions;
using SNS.Application.Profiles.SocialGraph.Commands.BlockProfile;
using SNS.Application.Profiles.SocialGraph.Commands.UnBlockProfile;
using SNS.Application.Profiles.SocialGraph.Queries.GetProfileBlockList;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Profiles.SocialGraph;

/// <summary>
/// Handles profile block and unblock relationships and block list retrieval operations.
/// </summary>
[Route("api/profiles/social-graph/[controller]")]
[ApiController]
[Produces("application/json")]
public class BlocksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BlocksController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Blocks a target profile, preventing social interaction and content sharing.
    /// </summary>
    /// <param name="targetProfileId">The unique identifier of the profile to block.</param>
    /// <response code="200">The profile was blocked successfully.</response>
    /// <response code="400">Cannot block own profile or invalid target ID.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="404">The target profile was not found.</response>
    [HttpPost("{targetProfileId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> BlockProfileAsync([FromRoute] Guid targetProfileId)
    {
        return (await _mediator.Send(new BlockProfileCommand(targetProfileId))).ToActionResult(this);
    }

    /// <summary>
    /// Unblocks a previously blocked profile.
    /// </summary>
    /// <param name="targetProfileId">The unique identifier of the profile to unblock.</param>
    /// <response code="200">The profile was unblocked successfully.</response>
    /// <response code="400">No active block relationship exists with target profile.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="404">The target profile was not found.</response>
    [HttpDelete("{targetProfileId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> UnBlockProfileAsync([FromRoute] Guid targetProfileId)
    {
        return (await _mediator.Send(new UnBlockProfileCommand(targetProfileId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paged list of profiles blocked by the authenticated user.
    /// </summary>
    /// <param name="query">The search term and pagination query parameters.</param>
    /// <response code="200">Returns paged collection of blocked profiles <see cref="BlockedProfileDto"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Paged<BlockedProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<Paged<BlockedProfileDto>>>> GetProfileBlockListAsync([FromQuery] SearchQueryFilter query)
    {
        return (await _mediator.Send(new GetProfileBlockListQuery(
            SearchTerm: query.SearchTerm,
            CurrentPage: query.CurrentPage,
            PageSize: query.PageSize))).ToActionResult(this);
    }
}

