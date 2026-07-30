using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.ContentManagement.Posts.Contracts;
using SNS.Application.ContentManagement.Posts.Queries.GetFeed;
using SNS.Shared.Results;

namespace SNS.API.Controllers.ContentManagment;

/// <summary>
/// Handles user content feed retrieval and post management operations.
/// </summary>
[Route("api/v{version:apiVersion}/content-managment/[controller]")]
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

    /// <summary>
    /// Retrieves a personalized paged content feed for the authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires authentication. Compiles top posts from followed profiles, joined communities, and relevant topics.
    /// </remarks>
    /// <param name="CurrentPage">The page index for pagination (1-based).</param>
    /// <param name="PageSize">The maximum number of post items to return per page.</param>
    /// <response code="200">Returns personalized feed post overviews <see cref="PostOverviewDto"/>.</response>
    /// <response code="401">The user is not authenticated.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("feed")]
    [ProducesResponseType(typeof(List<PostOverviewDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<List<PostOverviewDto>>>> GetFeedAsync([FromQuery] int CurrentPage = 1, int PageSize = 10)
    {
        return (await _mediator.Send(new GetFeedQuery(CurrentPage: CurrentPage, PageSize: PageSize))).ToActionResult(this);
    }
}

