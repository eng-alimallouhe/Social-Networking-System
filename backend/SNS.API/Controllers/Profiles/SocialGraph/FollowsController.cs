using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.DTOs.Shared;
using SNS.API.Extensions;
using SNS.Application.Profiles.SocialGraph.Commands.FollowProfile;
using SNS.Application.Profiles.SocialGraph.Commands.MuteProfile;
using SNS.Application.Profiles.SocialGraph.Commands.UnfollowProfile;
using SNS.Application.Profiles.SocialGraph.Commands.UnMuteProfile;
using SNS.Application.Profiles.SocialGraph.Contracts;
using SNS.Application.Profiles.SocialGraph.Queries.GetProfileFollowers;
using SNS.Application.Profiles.SocialGraph.Queries.GetProfileFollowings;
using SNS.Application.Shared.DTOs;
using SNS.Domain.Shared.Enums;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Profiles.SocialGraph;

[Route("api/profiles/social-graph/[controller]")]
[ApiController]
public class FollowsController : ControllerBase
{
    private readonly IMediator _mediator;

    public FollowsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("{targetProfileId:guid}")]
    public async Task<ActionResult<Result>> FollowProfileAsync([FromRoute] Guid targetProfileId)
    {
        return (await _mediator.Send(new FollowProfileCommand(targetProfileId))).ToActionResult(this);
    }

    [HttpDelete("{targetProfileId:guid}")]
    public async Task<ActionResult<Result>> UnFollowProfileAsync([FromRoute] Guid targetProfileId)
    {
        return (await _mediator.Send(new UnfollowProfileCommand(targetProfileId))).ToActionResult(this);
    }

    

    [HttpPost("{targetProfileId:guid}/mute")]
    public async Task<ActionResult<Result>> MuteProfileAsync([FromRoute] Guid targetProfileId, [FromQuery]TimePeriod period)
    {
        return (await _mediator.Send(new MuteProfileCommand(targetProfileId, period))).ToActionResult(this);
    }

    [HttpDelete("{targetProfileId:guid}/mute")]
    public async Task<ActionResult<Result>> UnMuteProfileAsync([FromRoute] Guid targetProfileId)
    {
        return (await _mediator.Send(new UnMuteProfileCommand(targetProfileId))).ToActionResult(this);
    }

    [HttpGet("{profileId:guid}/followers")]
    public async Task<ActionResult<Result<Paged<ProfileFollowDto>>>> GetProfileFollowersAsync([FromRoute] Guid profileId, [FromQuery] SearchQueryFilter query)
    {
        return (await _mediator.Send(new GetProfileFollowersQuery(
            ProfileId: profileId,
            SearchTerm: query.SearchTerm,
            CurrentPage: query.CurrentPage,
            PageSize: query.PageSize))).ToActionResult(this);
    }

    [HttpGet("{profileId:guid}/followings")]
    public async Task<ActionResult<Result<Paged<ProfileFollowDto>>>> GetProfileFollowingsAsync([FromRoute] Guid profileId, [FromQuery] SearchQueryFilter query)
    {
        return (await _mediator.Send(new GetProfileFollowingsQuery(
            ProfileId: profileId,
            SearchTerm: query.SearchTerm,
            CurrentPage: query.CurrentPage,
            PageSize: query.PageSize))).ToActionResult(this);
    }
}