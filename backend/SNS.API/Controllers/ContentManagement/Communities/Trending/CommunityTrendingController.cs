using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.ContentManagement.Communities.Communities.Contracts;
using SNS.Application.ContentManagement.Communities.Trending.Queries.GetTrendingCommunities;
using SNS.Shared.Results;

namespace SNS.API.Controllers.ContentManagement.Communities.Trending;

/// <summary>
/// Handles retrieving trending communities ordered by user activity and engagement metrics.
/// </summary>
[Route("api/v{version:apiVersion}/content-managment/communities/trending")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class CommunityTrendingController : ControllerBase
{
    private readonly IMediator _mediator;

    public CommunityTrendingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves top trending communities ordered by score.
    /// </summary>
    /// <param name="count">The maximum number of trending communities to return (default 10).</param>
    /// <response code="200">Returns list of trending communities <see cref="List{CommunitySummaryDto}"/>.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Result<List<CommunitySummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<List<CommunitySummaryDto>>>> GetTrendingCommunitiesAsync([FromQuery] int count = 10)
    {
        return (await _mediator.Send(new GetTrendingCommunitiesQuery(count))).ToActionResult(this);
    }
}
