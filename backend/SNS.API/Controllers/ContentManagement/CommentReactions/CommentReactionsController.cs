using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Contracts.ContentManagement;
using SNS.API.Extensions;
using SNS.Application.ContentManagement.Comments.CommentReactions.Commands.AddOrChangeCommentReaction;
using SNS.Application.ContentManagement.Comments.CommentReactions.Commands.RemoveCommentReaction;
using SNS.Application.ContentManagement.Comments.CommentReactions.Contracts;
using SNS.Application.ContentManagement.Comments.CommentReactions.Queries.GetCommentReactions;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.ContentManagement.CommentReactions;

[Route("api/v{version:apiVersion}/content-managment/comments/{commentId:guid}/reactions")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class CommentReactionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentReactionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Result<Paged<CommentReactionSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<Paged<CommentReactionSummaryDto>>>> GetCommentReactionsAsync(
        [FromRoute] Guid commentId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        return (await _mediator.Send(new GetCommentReactionsQuery(commentId, page, pageSize))).ToActionResult(this);
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [RequireSession]
    public async Task<ActionResult<Result>> AddOrChangeReactionAsync(
        [FromRoute] Guid commentId,
        [FromBody] ReactionRequest request)
    {
        return (await _mediator.Send(new AddOrChangeCommentReactionCommand(commentId, request.Type))).ToActionResult(this);
    }

    [HttpDelete]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [RequireSession]
    public async Task<ActionResult<Result>> RemoveReactionAsync([FromRoute] Guid commentId)
    {
        return (await _mediator.Send(new RemoveCommentReactionCommand(commentId))).ToActionResult(this);
    }
}
