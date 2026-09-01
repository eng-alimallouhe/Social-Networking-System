using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.Search.ContentManagement.Communitites.Queries.GetCommunitiesSearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Search.ContentManagement.Communities;

/// <summary>
/// Handles search operations for communities.
/// </summary>
[Route("api/v{version:apiVersion}/search/communities")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class CommunitiesSearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommunitiesSearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Searches communities based on keywords, type, and pagination.
    /// </summary>
    /// <param name="query">The community search query parameters.</param>
    /// <response code="200">Returns paginated community summary search results <see cref="SearchResult{CommunitySummaryDto}"/>.</response>
    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<SearchResult<CommunitySummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<SearchResult<CommunitySummaryDto>>>> SearchCommunitiesAsync([FromQuery] GetCommunitiesSearchQuery query)
    {
        return (await _mediator.Send(query)).ToActionResult(this);
    }
}
