using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Jobs.CompanyCreateRequests.Commands.ApproveCompanyCreateRequest;
using SNS.Application.Jobs.CompanyCreateRequests.Commands.CancelCompanyCreateRequest;
using SNS.Application.Jobs.CompanyCreateRequests.Commands.CreateCompanyCreateRequest;
using SNS.Application.Jobs.CompanyCreateRequests.Commands.RejectCompanyCreateRequest;
using SNS.Application.Jobs.CompanyCreateRequests.Contracts;
using SNS.Application.Jobs.CompanyCreateRequests.Queries.GetCompanyCreateRequestById;
using SNS.Application.Jobs.CompanyCreateRequests.Queries.GetMyCompanyCreateRequests;
using SNS.Application.Jobs.CompanyCreateRequests.Queries.GetPendingCompanyCreateRequests;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Jobs.CompanyCreateRequests;

/// <summary>
/// Payload containing optional review notes when approving or rejecting a company creation request.
/// </summary>
/// <param name="ReviewNote">Optional feedback or rationale for the decision.</param>
public sealed record ReviewCompanyCreateRequestRequest(string? ReviewNote = null);

/// <summary>
/// Handles submitting, cancelling, reviewing, and querying company creation requests.
/// </summary>
[Route("api/v{version:apiVersion}/company-create-requests")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class CompanyCreateRequestsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompanyCreateRequestsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Submits a new company creation request for administrative approval.
    /// </summary>
    /// <param name="command">Company request details.</param>
    /// <response code="201">Request successfully submitted.</response>
    /// <response code="400">Invalid parameters or duplicate pending request.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result<Guid>>> CreateCompanyCreateRequestAsync([FromBody] CreateCompanyCreateRequestCommand command)
    {
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves full details of a specific company creation request.
    /// </summary>
    /// <param name="requestId">The unique identifier of the request.</param>
    /// <response code="200">Request details retrieved successfully.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="404">Request not found.</response>
    [HttpGet("{requestId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result<CompanyCreateRequestDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result<CompanyCreateRequestDetailsDto>>> GetCompanyCreateRequestByIdAsync([FromRoute] Guid requestId)
    {
        return (await _mediator.Send(new GetCompanyCreateRequestByIdQuery(requestId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves all company creation requests submitted by the authenticated user.
    /// </summary>
    /// <response code="200">User's requests retrieved successfully.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpGet("my-requests")]
    [Authorize]
    [ProducesResponseType(typeof(Result<List<CompanyCreateRequestSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result<List<CompanyCreateRequestSummaryDto>>>> GetMyCompanyCreateRequestsAsync()
    {
        return (await _mediator.Send(new GetMyCompanyCreateRequestsQuery())).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paginated list of pending company creation requests awaiting moderation/review.
    /// </summary>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="currentPage">Current page index.</param>
    /// <response code="200">Pending requests retrieved successfully.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    [HttpGet("pending")]
    [Authorize]
    [ProducesResponseType(typeof(Result<Paged<CompanyCreateRequestSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [RequireSession]
    public async Task<ActionResult<Result<Paged<CompanyCreateRequestSummaryDto>>>> GetPendingCompanyCreateRequestsAsync(
        [FromQuery] int pageSize = 10,
        [FromQuery] int currentPage = 1)
    {
        return (await _mediator.Send(new GetPendingCompanyCreateRequestsQuery(pageSize, currentPage))).ToActionResult(this);
    }

    /// <summary>
    /// Cancels a pending company creation request submitted by the authenticated user.
    /// </summary>
    /// <param name="requestId">The unique identifier of the request.</param>
    /// <response code="200">Request successfully cancelled.</response>
    /// <response code="400">Request is not pending.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user is not the owner of the request.</response>
    /// <response code="404">Request not found.</response>
    [HttpPatch("{requestId:guid}/cancel")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> CancelCompanyCreateRequestAsync([FromRoute] Guid requestId)
    {
        return (await _mediator.Send(new CancelCompanyCreateRequestCommand(requestId))).ToActionResult(this);
    }

    /// <summary>
    /// Approves a company creation request, automatically creating the company and assigning the requester as owner.
    /// </summary>
    /// <param name="requestId">The unique identifier of the request.</param>
    /// <param name="request">Optional review note.</param>
    /// <response code="200">Request successfully approved and company created.</response>
    /// <response code="400">Request is not pending.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Request not found.</response>
    [HttpPost("{requestId:guid}/approve")]
    [Authorize]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result<Guid>>> ApproveCompanyCreateRequestAsync(
        [FromRoute] Guid requestId,
        [FromBody] ReviewCompanyCreateRequestRequest? request = null)
    {
        return (await _mediator.Send(new ApproveCompanyCreateRequestCommand(requestId, request?.ReviewNote))).ToActionResult(this);
    }

    /// <summary>
    /// Rejects a pending company creation request.
    /// </summary>
    /// <param name="requestId">The unique identifier of the request.</param>
    /// <param name="request">Optional review note describing reasons for rejection.</param>
    /// <response code="200">Request successfully rejected.</response>
    /// <response code="400">Request is not pending.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden.</response>
    /// <response code="404">Request not found.</response>
    [HttpPost("{requestId:guid}/reject")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> RejectCompanyCreateRequestAsync(
        [FromRoute] Guid requestId,
        [FromBody] ReviewCompanyCreateRequestRequest? request = null)
    {
        return (await _mediator.Send(new RejectCompanyCreateRequestCommand(requestId, request?.ReviewNote))).ToActionResult(this);
    }
}
