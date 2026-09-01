using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Search.Jobs.Contracts;
using SNS.Application.Search.Jobs.Queries.GetJobsSearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Search.Jobs;

/// <summary>
/// Handles search operations for job postings.
/// </summary>
[Route("api/v{version:apiVersion}/search/jobs")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class JobsSearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobsSearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Searches job postings based on keywords, employment type, salary structure, salary range, date range, and pagination.
    /// </summary>
    /// <param name="query">The job search query parameters.</param>
    /// <response code="200">Returns paginated job summary search results <see cref="SearchResult{JobSummaryDto}"/>.</response>
    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<SearchResult<JobSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<SearchResult<JobSummaryDto>>>> SearchJobsAsync([FromQuery] GetJobsSearchQuery query)
    {
        return (await _mediator.Send(query)).ToActionResult(this);
    }
}
