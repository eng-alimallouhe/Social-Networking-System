using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.ContentManagement.Posts.Posts.Commands.CreatePost;
using SNS.Application.ContentManagement.Posts.Posts.Commands.DecreaseInterest;
using SNS.Application.ContentManagement.Posts.Posts.Commands.DeletePost;
using SNS.Application.ContentManagement.Posts.Posts.Commands.IncreaseInterest;
using SNS.Application.ContentManagement.Posts.Posts.Commands.UpdatePost;
using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.ContentManagement.Posts.Posts.Queries.GetFeed;
using SNS.Application.ContentManagement.Posts.Posts.Queries.GetPostById;
using SNS.Application.ContentManagement.Posts.Posts.Queries.GetUserPosts;
using SNS.Application.ContentManagement.Posts.Posts.Queries.GetUserReactedPosts;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;

namespace SNS.API.Controllers.ContentManagement.Posts;

[Route("api/v{version:apiVersion}/content-managment/Posts")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class PostsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result>> CreatePostAsync([FromBody] CreatePostCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpGet("feed")]
    [ProducesResponseType(typeof(Result<List<PostOverviewDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<List<PostOverviewDto>>>> GetFeedAsync([FromQuery] GetFeedQuery query)
    {
        return (await _mediator.Send(query)).ToActionResult(this);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Result<PostDetailsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<PostDetailsDto>>> GetPostByIdAsync([FromRoute] Guid id)
    {
        return (await _mediator.Send(new GetPostByIdQuery(id))).ToActionResult(this);
    }

    [HttpGet("user/{profileId:guid}")]
    [ProducesResponseType(typeof(Result<Paged<PostOverviewDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<Paged<PostOverviewDto>>>> GetUserPostsAsync(
        [FromRoute] Guid profileId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return (await _mediator.Send(new GetUserPostsQuery(profileId, page, pageSize))).ToActionResult(this);
    }

    [HttpGet("reacted")]
    [Authorize]
    [ProducesResponseType(typeof(Result<Paged<PostOverviewDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<Paged<PostOverviewDto>>>> GetUserReactedPostsAsync([FromQuery] GetUserReactedPostsQuery query)
    {
        return (await _mediator.Send(query)).ToActionResult(this);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result>> UpdatePostAsync(
        [FromRoute] Guid id,
        [FromBody] UpdatePostCommand request)
    {
        var command = request with { PostId = id };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result>> DeletePostAsync([FromRoute] Guid id)
    {
        return (await _mediator.Send(new DeletePostCommand(id))).ToActionResult(this);
    }

    [HttpPost("{id:guid}/interest/increase")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result>> IncreaseInterestAsync([FromRoute] Guid id)
    {
        return (await _mediator.Send(new IncreasePostInterestCommand(id))).ToActionResult(this);
    }

    [HttpPost("{id:guid}/interest/decrease")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result>> DecreaseInterestAsync([FromRoute] Guid id)
    {
        return (await _mediator.Send(new DecreasePostInterestCommand(id))).ToActionResult(this);
    }
}
