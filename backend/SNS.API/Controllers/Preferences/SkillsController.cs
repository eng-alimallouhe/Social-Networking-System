using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Preferences.Skills.Queries.GetSkills;
using SNS.Application.Preferences.Skills.Contracts;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Preferences;

/// <summary>
/// Handles skill retrieval and autocomplete operations.
/// </summary>
[Route("api/v{version:apiVersion}/preferences/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class SkillsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SkillsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves skills matching the optional search term for autocomplete (at most 10 matching skills).
    /// </summary>
    /// <param name="search">Optional search keyword to filter skills by name.</param>
    /// <response code="200">Returns the list of matching skills <see cref="SkillDto"/>.</response>
    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(List<SkillDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<List<SkillDto>>>> GetSkillsAsync([FromQuery] string? search = null)
    {
        return (await _mediator.Send(new GetSkillsQuery(search))).ToActionResult(this);
    }
}
