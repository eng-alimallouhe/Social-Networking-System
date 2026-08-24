using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.Users.UsersManagement.Queries.GetUserSecurityDetails;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.SecuritySettings;

/// <summary>
/// Handles user general security settings management.
/// </summary>
[Route("api/v{version:apiVersion}/identity/[controller]")]
[ApiVersion("1.0")]
[Authorize]
[ApiController]
[Produces("application/json")]
public class SecuritySettingsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SecuritySettingsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves security settings configuration status for the authenticated user.
    /// </summary>
    /// <response code="200">Returns current security settings configuration.</response>
    /// <response code="401">The user is not authenticated.</response>
    [HttpGet("user-security-settings")]
    [ProducesResponseType(typeof(UserSecurityDetailsResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<UserSecurityDetailsResult>>> GetUserSecuritySettingsAsync()
    {
        return (await _mediator.Send(new GetUserSecurityDetailsQuery())).ToActionResult(this);
    }
}
