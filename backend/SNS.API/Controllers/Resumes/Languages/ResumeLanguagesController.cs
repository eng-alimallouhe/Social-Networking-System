using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Resumes.Languages.Commands.AddResumeLanguage;
using SNS.Application.Resumes.Languages.Commands.DeleteResumeLanguage;
using SNS.Application.Resumes.Languages.Commands.UpdateResumeLanguage;
using SNS.Application.Resumes.Languages.Contracts;
using SNS.Application.Resumes.Languages.Queries.GetResumeLanguages;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Resumes.Languages;

/// <summary>
/// Handles language proficiency entries for a resume.
/// </summary>
[Route("api/v{version:apiVersion}/resumes/{resumeId:guid}/languages")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ResumeLanguagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResumeLanguagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all language proficiency entries for a specific resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <response code="200">Returns the list of language proficiency records.</response>
    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<List<ResumeLanguageDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<List<ResumeLanguageDto>>>> GetResumeLanguagesAsync([FromRoute] Guid resumeId)
    {
        return (await _mediator.Send(new GetResumeLanguagesQuery(resumeId))).ToActionResult(this);
    }

    /// <summary>
    /// Adds a new language proficiency entry to a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="request">The language creation payload.</param>
    /// <response code="201">Returns the identifier of the created language record.</response>
    /// <response code="400">The provided language data is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not the owner of the resume.</response>
    /// <response code="404">The resume was not found.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result<Guid>>> AddResumeLanguageAsync(
        [FromRoute] Guid resumeId,
        [FromBody] AddResumeLanguageCommand request)
    {
        var command = request with { ResumeId = resumeId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Updates an existing language proficiency entry on a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="languageId">The unique identifier of the language entry to update.</param>
    /// <param name="request">The updated language payload.</param>
    /// <response code="200">The language entry was updated successfully.</response>
    /// <response code="400">The provided payload is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not the owner of the resume.</response>
    /// <response code="404">The resume or language entry was not found.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("{languageId:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> UpdateResumeLanguageAsync(
        [FromRoute] Guid resumeId,
        [FromRoute] Guid languageId,
        [FromBody] UpdateResumeLanguageCommand request)
    {
        var command = request with { ResumeId = resumeId, LanguageId = languageId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Deletes a language proficiency entry from a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="languageId">The unique identifier of the language entry to delete.</param>
    /// <response code="200">The language entry was deleted successfully.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not the owner of the resume.</response>
    /// <response code="404">The resume or language entry was not found.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpDelete("{languageId:guid}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> DeleteResumeLanguageAsync(
        [FromRoute] Guid resumeId,
        [FromRoute] Guid languageId)
    {
        return (await _mediator.Send(new DeleteResumeLanguageCommand(resumeId, languageId))).ToActionResult(this);
    }
}
