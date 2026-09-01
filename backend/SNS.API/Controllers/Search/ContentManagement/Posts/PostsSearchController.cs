using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.ContentManagement.Posts.Posts.Contracts;
using SNS.Application.Search.ContentManagement.Posts.Queries.GetPostsSearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Search.ContentManagement.Posts;

/// <summary>
/// Handles search operations for content posts.
/// </summary>
[Route("api/v{version:apiVersion}/search/posts")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class PostsSearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public PostsSearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Searches posts based on keywords, date range, tags, topics, and pagination.
    /// </summary>
    /// <param name="query">The post search query parameters.</param>
    /// <response code="200">Returns paginated post overview search results <see cref="SearchResult{PostOverviewDto}"/>.</response>
    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<SearchResult<PostOverviewDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<SearchResult<PostOverviewDto>>>> SearchPostsAsync([FromQuery] GetPostsSearchQuery query)
    {
        return (await _mediator.Send(query)).ToActionResult(this);
    }
}
