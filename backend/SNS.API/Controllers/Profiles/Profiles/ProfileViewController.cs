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

namespace SNS.API.Controllers.Profiles.Profiles;

[Route("api/v{version:apiVersion}/profiles/[controller]")]
[ApiVersion("1.0")]
[ApiController]
public class ProfileViewController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileViewController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    public async Task<ActionResult<Result>> ViewProfileAsync([FromBody] ViewProfileCommand command, CancellationToken cancellationToken)
    {
        return (await _mediator.Send(command, cancellationToken)).ToActionResult(this);
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost("batch")]
    public async Task<ActionResult<Result>> ViewProfilesAsync([FromBody] ViewProfilesCommand command, CancellationToken cancellationToken)
    {
        return (await _mediator.Send(command, cancellationToken)).ToActionResult(this);
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("viewed-profiles")]
    public async Task<ActionResult<Result<Paged<ProfileViewDto>>>> GetViewedProfilesAsync([FromQuery] GetViewedProfilesQuery query)
    {
        return (await _mediator.Send(query)).ToActionResult(this);
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("viewers")]
    public async Task<ActionResult<Result<Paged<ProfileViewDto>>>> GetProfileViewersAsync([FromQuery] GetProfileViewersQuery query)
    {
        return (await _mediator.Send(query)).ToActionResult(this);
    }
}