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

[Route("api/profiles/social-graph/[controller]")]
[ApiController]
public class BlocksController : ControllerBase
{
    private readonly IMediator _mediator;

    public BlocksController(IMediator mediator)
    {
        _mediator = mediator;
    }


    [HttpPost("{targetProfileId:guid}")]
    public async Task<ActionResult<Result>> BlockProfileAsync([FromRoute] Guid targetProfileId)
    {
        return (await _mediator.Send(new BlockProfileCommand(targetProfileId))).ToActionResult(this);
    }

    [HttpDelete("{targetProfileId:guid}")]
    public async Task<ActionResult<Result>> UnBlockProfileAsync([FromRoute] Guid targetProfileId)
    {
        return (await _mediator.Send(new UnBlockProfileCommand(targetProfileId))).ToActionResult(this);
    }


    [HttpGet]
    public async Task<ActionResult<Result<Paged<BlockedProfileDto>>>> GetProfileBlockListAsync([FromQuery] SearchQueryFilter query)
    {
        return (await _mediator.Send(new GetProfileBlockListQuery(
            SearchTerm: query.SearchTerm,
            CurrentPage: query.CurrentPage,
            PageSize: query.PageSize))).ToActionResult(this);
    }
}
