using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Contracts.ContentManagement;
using SNS.API.Extensions;
using SNS.Application.ContentManagement.Posts.PostReactions.Commands.AddOrChangePostReaction;
using SNS.Application.ContentManagement.Posts.PostReactions.Commands.RemovePostReaction;
using SNS.Application.ContentManagement.Posts.PostReactions.Contracts;
using SNS.Application.ContentManagement.Posts.PostReactions.Queries.GetPostReactions;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.ContentManagement.Posts;

[Route("api/v{version:apiVersion}/content-managment/posts/{postId:guid}/reactions")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class PostReactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostReactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<Paged<PostReactionSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<Paged<PostReactionSummaryDto>>>> GetPostReactionsAsync(
        [FromRoute] Guid postId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return (await _mediator.Send(new GetPostReactionsQuery(postId, page, pageSize))).ToActionResult(this);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [RequireSession]
    public async Task<ActionResult<Result>> AddOrChangeReactionAsync(
        [FromRoute] Guid postId,
        [FromBody] ReactionRequest request)
    {
        return (await _mediator.Send(new AddOrChangePostReactionCommand(postId, request.Type))).ToActionResult(this);
    }

    [HttpDelete]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [RequireSession]
    public async Task<ActionResult<Result>> RemoveReactionAsync([FromRoute] Guid postId)
    {
        return (await _mediator.Send(new RemovePostReactionCommand(postId))).ToActionResult(this);
    }
}
