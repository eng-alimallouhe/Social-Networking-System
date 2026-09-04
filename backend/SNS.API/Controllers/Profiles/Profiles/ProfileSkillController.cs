using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Profiles.Profiles.Commands.AddSkillToProfile;
using SNS.Application.Profiles.Profiles.Commands.RemoveSkillFromProfile;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Profiles.Profiles;

/// <summary>
/// Handles adding and removing professional skills on a user profile.
/// </summary>
[Route("api/v{version:apiVersion}/profiles/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProfileSkillController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileSkillController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Adds a professional skill with proficiency level to the authenticated user's profile.
    /// </summary>
    /// <param name="request">The skill addition payload containing skill ID and proficiency level.</param>
    /// <response code="200">The skill was added to profile successfully.</response>
    /// <response code="400">The skill request parameters are invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="409">The skill has already been added to the profile.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [RequireSession]
    public async Task<ActionResult<Result>> AddSkillToProfileAsync([FromBody] AddSkillToProfileCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Removes a skill association from the authenticated user's profile.
    /// </summary>
    /// <param name="request">The command payload specifying the skill association ID to remove.</param>
    /// <response code="200">The skill was removed from profile successfully.</response>
    /// <response code="400">The skill association ID is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="404">The specified skill association was not found on the profile.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpDelete]
    [Consumes("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> RemoveSkillFromProfileAsync([FromBody] RemoveSkillFromProfileCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }
}

