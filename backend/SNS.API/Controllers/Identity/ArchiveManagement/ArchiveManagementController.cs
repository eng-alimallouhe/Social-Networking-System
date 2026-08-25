using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.ArchiveManagement.Commands.ExportAccountData;
using SNS.Application.Identity.ArchiveManagement.Contracts;
using SNS.Application.Identity.ArchiveManagement.Qureies.GetUserArchive;
using SNS.Application.Identity.ArchiveManagement.Qureies.GetUserIdentityArchive;
using SNS.Application.Identity.ArchiveManagement.Qureies.GetUserPasswordArchive;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.ArchiveManagement;

/// <summary>
/// Manages user historical archive records and personal account data exports.
/// </summary>
[Route("api/v{version:apiVersion}/identity/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class ArchiveManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArchiveManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves paged historical user archive summaries for the authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires authentication. Returns general user archive audit records.
    /// </remarks>
    /// <param name="request">The query pagination and filtering criteria.</param>
    /// <response code="200">Returns paged collection of <see cref="UserArchiveSummaryDto"/>.</response>
    /// <response code="401">The request is unauthenticated.</response>
    [Authorize]
    [HttpGet("user-archive")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Paged<UserArchiveSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<Paged<UserArchiveSummaryDto>>>> GetUserArchiveAsync([FromQuery] GetUserArchiveQuery request)
    {
        var result = await _mediator.Send(request);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Retrieves paged user identity archive history for the authenticated user.
    /// </summary>
    /// <remarks>
    /// Requires authentication. Tracks changes to profile and identity identifiers over time.
    /// </remarks>
    /// <param name="request">The query pagination and filtering criteria.</param>
    /// <response code="200">Returns paged collection of identity archive summaries <see cref="UserArchiveSummaryDto"/>.</response>
    /// <response code="401">The request is unauthenticated.</response>
    [Authorize]
    [HttpGet("user-identity-archive")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Paged<UserArchiveSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<Paged<UserIdentityArchiveSummaryDto>>>> GetUserIdentityArchiveAsync([FromQuery] GetUserIdentityArchiveQuery request)
    {
        var result = await _mediator.Send(request);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Retrieves paged user password change history archives.
    /// </summary>
    /// <remarks>
    /// Requires authentication. Audits historical password changes and timestamp metadata.
    /// </remarks>
    /// <param name="request">The query pagination parameters.</param>
    /// <response code="200">Returns paged collection of password archive summaries <see cref="UserPasswordArchiveSummaryDto"/>.</response>
    /// <response code="401">The request is unauthenticated.</response>
    [Authorize]
    [HttpGet("user-password-archive")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(Paged<UserPasswordArchiveSummaryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<Paged<UserPasswordArchiveSummaryDto>>>> GetUserIdentityArchiveAsync([FromQuery] GetUserPasswordArchiveQuery request)
    {
        var result = await _mediator.Send(request);
        return result.ToActionResult(this);
    }

    /// <summary>
    /// Initiates an asynchronous personal account data export package generation request.
    /// </summary>
    /// <remarks>
    /// Requires authentication. Asynchronously compiles all personal data into a downloadable archive.
    /// </remarks>
    /// <param name="request">The export request parameters.</param>
    /// <response code="200">Returns export status and request confirmation <see cref="ExportAccountDataResponseDto"/>.</response>
    /// <response code="400">Export request cannot be initiated (e.g., active request already pending).</response>
    /// <response code="401">The request is unauthenticated.</response>
    [Authorize]
    [HttpPost("export-account-data")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(ExportAccountDataResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<ExportAccountDataResponseDto>>> ExportAccountDataAsync()
    {
        return (await _mediator.Send(new ExportAccountDataCommand())).ToActionResult(this);
    }
}

