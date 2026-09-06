using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Preferences.Tags.Queries.GetTags;
using SNS.Application.Preferences.Tags.Contracts;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Preferences;

/// <summary>
/// Handles tag retrieval and autocomplete operations.
/// </summary>
[Route("api/v{version:apiVersion}/tags")]
[Route("api/v{version:apiVersion}/preferences/[controller]")]
[Route("api/v{version:apiVersion}/content-managment/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class TagsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TagsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves tags matching the optional search term for autocomplete (at most 10 matching tags).
    /// </summary>
    /// <param name="search">Optional search keyword to filter tags by name.</param>
    /// <response code="200">Returns the list of matching tags <see cref="TagDto"/>.</response>
    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(List<TagDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<List<TagDto>>>> GetTagsAsync([FromQuery] string? search = null)
    {
        return (await _mediator.Send(new GetTagsQuery(search))).ToActionResult(this);
    }
}
