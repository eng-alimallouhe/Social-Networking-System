using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Profiles.Profiles.Commands.ViewProfile;
using SNS.Application.Profiles.Profiles.Commands.ViewProfiles;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Profiles.Profiles.Queries.GetProfileViewers;
using SNS.Application.Profiles.Profiles.Queries.GetViewedProfiles;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Profiles.Profiles;

/// <summary>
/// Handles profile view tracking, batch view logging, and history of profile viewers and viewed profiles.
/// </summary>
[Route("api/v{version:apiVersion}/profiles/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProfileViewController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileViewController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Logs a single profile view interaction event.
    /// </summary>
    /// <param name="command">The command containing the target viewed profile ID.</param>
    /// <param name="cancellationToken">Cancellation token for cancelling request operation.</param>
    /// <response code="200">The profile view event was logged successfully.</response>
    /// <response code="400">The target profile ID is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result>> ViewProfileAsync([FromBody] ViewProfileCommand command, CancellationToken cancellationToken)
    {
        return (await _mediator.Send(command, cancellationToken)).ToActionResult(this);
    }

    /// <summary>
    /// Logs a batch of profile view events (e.g. from feed or search results).
    /// </summary>
    /// <param name="command">The command containing the list of viewed profile IDs.</param>
    /// <param name="cancellationToken">Cancellation token for cancelling request operation.</param>
    /// <response code="200">The batch profile view events were logged successfully.</response>
    /// <response code="400">The profile IDs list is empty or invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost("batch")]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result>> ViewProfilesAsync([FromBody] ViewProfilesCommand command, CancellationToken cancellationToken)
    {
        return (await _mediator.Send(command, cancellationToken)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paged list of profiles viewed by the authenticated user.
    /// </summary>
    /// <param name="query">The query options and pagination criteria.</param>
    /// <response code="200">Returns paged list of viewed profiles <see cref="ProfileViewDto"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("viewed-profiles")]
    [ProducesResponseType(typeof(Paged<ProfileViewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result<Paged<ProfileViewDto>>>> GetViewedProfilesAsync([FromQuery] GetViewedProfilesQuery query)
    {
        return (await _mediator.Send(query)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paged list of profiles that have viewed the authenticated user's profile.
    /// </summary>
    /// <param name="query">The query options and pagination criteria.</param>
    /// <response code="200">Returns paged list of profile viewers <see cref="ProfileViewDto"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("viewers")]
    [ProducesResponseType(typeof(Paged<ProfileViewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result<Paged<ProfileViewDto>>>> GetProfileViewersAsync([FromQuery] GetProfileViewersQuery query)
    {
        return (await _mediator.Send(query)).ToActionResult(this);
    }
}