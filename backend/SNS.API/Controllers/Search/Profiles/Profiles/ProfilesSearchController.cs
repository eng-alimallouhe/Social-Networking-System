using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Search.Profiles.Profiles.Queries.GetProfilesSearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Search.Profiles.Profiles;

/// <summary>
/// Handles search operations for user profiles.
/// </summary>
[Route("api/v{version:apiVersion}/search/profiles")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ProfilesSearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfilesSearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Searches user profiles by keyword, required skills, and pagination.
    /// </summary>
    /// <param name="query">The profile search query parameters.</param>
    /// <response code="200">Returns paginated profile summary search results <see cref="SearchResult{ProfileSummaryDto}"/>.</response>
    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<SearchResult<ProfileSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<SearchResult<ProfileSummaryDto>>>> SearchProfilesAsync([FromQuery] GetProfilesSearchQuery query)
    {
        return (await _mediator.Send(query)).ToActionResult(this);
    }
}
