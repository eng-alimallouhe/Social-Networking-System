using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Moderation.Commands.ReportPost;
using SNS.Domain.Moderation.Enums;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Moderation;

public record ReportPostRequest(ViolationReason Reason, string? Details);

/// <summary>
/// Handles content moderation and reporting operations.
/// </summary>
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ModerationController : ControllerBase
{
    private readonly IMediator _mediator;

    public ModerationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Reports a post for a violation.
    /// </summary>
    /// <param name="postId">The unique identifier of the post.</param>
    /// <param name="request">The report details.</param>
    /// <response code="200">Post reported successfully.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="404">The post was not found.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost("posts/{postId:guid}/report")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> ReportPostAsync(
        [FromRoute] Guid postId, 
        [FromBody] ReportPostRequest request)
    {
        return (await _mediator.Send(new ReportPostCommand(postId, request.Reason, request.Details))).ToActionResult(this);
    }
}
