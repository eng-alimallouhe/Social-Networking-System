using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Resumes.Certificates.Commands.AddResumeCertificate;
using SNS.Application.Resumes.Certificates.Commands.DeleteResumeCertificate;
using SNS.Application.Resumes.Certificates.Commands.UpdateResumeCertificate;
using SNS.Application.Resumes.Certificates.Contracts;
using SNS.Application.Resumes.Certificates.Queries.GetResumeCertificates;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Resumes.Certificates;

/// <summary>
/// Handles professional certificate operations for a resume.
/// </summary>
[Route("api/v{version:apiVersion}/resumes/{resumeId:guid}/certificates")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ResumeCertificatesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResumeCertificatesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all certificate entries for a specific resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <response code="200">Returns the list of certificate records.</response>
    [MapToApiVersion("1.0")]
    [HttpGet]
    [ProducesResponseType(typeof(Result<List<ResumeCertificateDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<List<ResumeCertificateDto>>>> GetResumeCertificatesAsync([FromRoute] Guid resumeId)
    {
        return (await _mediator.Send(new GetResumeCertificatesQuery(resumeId))).ToActionResult(this);
    }

    /// <summary>
    /// Adds a new certificate entry to a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="request">The certificate creation payload.</param>
    /// <response code="201">Returns the identifier of the created certificate record.</response>
    /// <response code="400">The provided certificate data is invalid.</response>
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
    public async Task<ActionResult<Result<Guid>>> AddResumeCertificateAsync(
        [FromRoute] Guid resumeId,
        [FromBody] AddResumeCertificateCommand request)
    {
        var command = request with { ResumeId = resumeId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Updates an existing certificate entry on a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="certificateId">The unique identifier of the certificate entry to update.</param>
    /// <param name="request">The updated certificate payload.</param>
    /// <response code="200">The certificate entry was updated successfully.</response>
    /// <response code="400">The provided payload is invalid.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not the owner of the resume.</response>
    /// <response code="404">The resume or certificate entry was not found.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("{certificateId:guid}")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> UpdateResumeCertificateAsync(
        [FromRoute] Guid resumeId,
        [FromRoute] Guid certificateId,
        [FromBody] UpdateResumeCertificateCommand request)
    {
        var command = request with { ResumeId = resumeId, CertificateId = certificateId };
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Deletes a certificate entry from a resume.
    /// </summary>
    /// <param name="resumeId">The unique identifier of the resume.</param>
    /// <param name="certificateId">The unique identifier of the certificate entry to delete.</param>
    /// <response code="200">The certificate entry was deleted successfully.</response>
    /// <response code="401">The user is not authenticated.</response>
    /// <response code="403">The user is not the owner of the resume.</response>
    /// <response code="404">The resume or certificate entry was not found.</response>
    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpDelete("{certificateId:guid}")]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> DeleteResumeCertificateAsync(
        [FromRoute] Guid resumeId,
        [FromRoute] Guid certificateId)
    {
        return (await _mediator.Send(new DeleteResumeCertificateCommand(resumeId, certificateId))).ToActionResult(this);
    }
}
