using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Profiles.Profiles.Commands.AddSkillToProfile;
using SNS.Application.Profiles.Profiles.Commands.RemoveSkillFromProfile;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Profiles.Profiles;

[Route("api/v{version:apiVersion}/profiles/[controller]")]
[ApiVersion("1.0")]
[ApiController]
public class ProfileSkillController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileSkillController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    public async Task<ActionResult<Result>> AddSkillToProfileAsync([FromBody] AddSkillToProfileCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }


    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpDelete]
    public async Task<ActionResult<Result>> RemoveSkillFromProfileAsync([FromBody] RemoveSkillFromProfileCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }
}
