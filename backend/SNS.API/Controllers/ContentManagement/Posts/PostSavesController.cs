using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.ContentManagement.Posts.PostSaves.Commands.SavePost;
using SNS.Application.ContentManagement.Posts.PostSaves.Commands.UnsavePost;
using SNS.Application.ContentManagement.Posts.PostSaves.Queries.GetSavedPosts;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.ContentManagement.Posts;

[Route("api/v{version:apiVersion}/content-managment/posts")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class PostSavesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostSavesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("saved")]
    [Authorize]
    [ProducesResponseType(typeof(Result<Paged<PostOverviewDto>>), StatusCodes.Status200OK)]
    [RequireSession]
    public async Task<ActionResult<Result<Paged<PostOverviewDto>>>> GetSavedPostsAsync([FromQuery] GetSavedPostsQuery query)
    {
        return (await _mediator.Send(query)).ToActionResult(this);
    }

    [HttpPost("{postId:guid}/save")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [RequireSession]
    public async Task<ActionResult<Result>> SavePostAsync([FromRoute] Guid postId)
    {
        return (await _mediator.Send(new SavePostCommand(postId))).ToActionResult(this);
    }

    [HttpDelete("{postId:guid}/save")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [RequireSession]
    public async Task<ActionResult<Result>> UnsavePostAsync([FromRoute] Guid postId)
    {
        return (await _mediator.Send(new UnsavePostCommand(postId))).ToActionResult(this);
    }
}
