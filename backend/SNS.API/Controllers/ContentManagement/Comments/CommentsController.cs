using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.ContentManagement.Comments.Comments.Commands.CreateComment;
using SNS.Application.ContentManagement.Comments.Comments.Commands.DeleteComment;
using SNS.Application.ContentManagement.Comments.Comments.Commands.UpdateComment;
using SNS.Application.ContentManagement.Comments.Comments.Contracts;
using SNS.Application.ContentManagement.Comments.Comments.Queries.GetCommentById;
using SNS.Application.ContentManagement.Comments.Comments.Queries.GetCommentReplies;
using SNS.Application.ContentManagement.Comments.Comments.Queries.GetPostComments;
using SNS.Application.ContentManagement.Comments.Comments.Queries.GetUserComments;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;

namespace SNS.API.Controllers.ContentManagement.Comments;

[Route("api/v{version:apiVersion}/content-managment/Comments")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result>> CreateCommentAsync([FromBody] CreateCommentCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result>> UpdateCommentAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateCommentCommand request)
    {
        var command = request with { CommentId = id };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result>> DeleteCommentAsync([FromRoute] Guid id)
    {
        return (await _mediator.Send(new DeleteCommentCommand(id))).ToActionResult(this);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(Result<CommentDetailsDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<CommentDetailsDto>>> GetCommentByIdAsync([FromRoute] Guid id)
    {
        return (await _mediator.Send(new GetCommentByIdQuery(id))).ToActionResult(this);
    }

    [HttpGet("post/{postId:guid}")]
    [ProducesResponseType(typeof(Result<Paged<CommentSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<Paged<CommentSummaryDto>>>> GetPostCommentsAsync(
        [FromRoute] Guid postId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return (await _mediator.Send(new GetPostCommentsQuery(postId, page, pageSize))).ToActionResult(this);
    }

    [HttpGet("{id:guid}/replies")]
    [ProducesResponseType(typeof(Result<Paged<CommentSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<Paged<CommentSummaryDto>>>> GetCommentRepliesAsync(
        [FromRoute] Guid id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return (await _mediator.Send(new GetCommentRepliesQuery(id, page, pageSize))).ToActionResult(this);
    }

    [HttpGet("user/{profileId:guid}")]
    [ProducesResponseType(typeof(Result<Paged<CommentSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<Paged<CommentSummaryDto>>>> GetUserCommentsAsync(
        [FromRoute] Guid profileId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return (await _mediator.Send(new GetUserCommentsQuery(profileId, page, pageSize))).ToActionResult(this);
    }

    [HttpGet("my-comments")]
    [Authorize]
    [ProducesResponseType(typeof(Result<Paged<CommentSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<Paged<CommentSummaryDto>>>> GetMyCommentsAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        return (await _mediator.Send(new GetUserCommentsQuery(null, page, pageSize))).ToActionResult(this);
    }
}
