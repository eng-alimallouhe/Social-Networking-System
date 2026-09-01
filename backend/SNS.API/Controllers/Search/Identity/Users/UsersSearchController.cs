using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.Shared.DTOs.Users;
using SNS.Application.Search.Identity.Users.Queries.GetUsersSearch;
using SNS.Application.Search.Shared.Contracts;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Search.Identity.Users;

/// <summary>
/// Handles search operations for user accounts in the platform.
/// </summary>
[Route("api/v{version:apiVersion}/search/users")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class UsersSearchController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersSearchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Searches user accounts based on filter, sorting, and pagination parameters.
    /// </summary>
    /// <param name="query">The user search query parameters.</param>
    /// <response code="200">Returns paginated user summary search results <see cref="SearchResult{UserSummaryDto}"/>.</response>
    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<SearchResult<UserSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<SearchResult<UserSummaryDto>>>> SearchUsersAsync([FromQuery] GetUsersSearchQuery query)
    {
        return (await _mediator.Send(query)).ToActionResult(this);
    }
}
