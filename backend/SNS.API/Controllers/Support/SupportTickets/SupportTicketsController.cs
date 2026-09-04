using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Shared.DTOs;
using SNS.Application.Support.SupportTickets.Commands.AssignSupportTicket;
using SNS.Application.Support.SupportTickets.Commands.ChangeSupportTicketPriority;
using SNS.Application.Support.SupportTickets.Commands.ChangeSupportTicketStatus;
using SNS.Application.Support.SupportTickets.Commands.CreateSupportTicket;
using SNS.Application.Support.SupportTickets.Contracts;
using SNS.Application.Support.SupportTickets.Queries.GetMySupportTickets;
using SNS.Application.Support.SupportTickets.Queries.GetSupportTicketById;
using SNS.Application.Support.SupportTickets.Queries.GetSupportTickets;
using SNS.Domain.Identity.Users.Constants;
using SNS.Domain.Support.Enums;
using SNS.Infrastructure.Identity.Shared.Authorization;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Support.SupportTickets;

/// <summary>
/// Handles operations for creating, assigning, updating status/priority, and querying customer support tickets.
/// </summary>
[Route("api/v{version:apiVersion}/support/tickets")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class SupportTicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public SupportTicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new support ticket with an initial message and optional attachments.
    /// </summary>
    /// <param name="request">Ticket details.</param>
    /// <response code="201">Ticket created successfully.</response>
    /// <response code="400">Invalid ticket data.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result<Guid>>> CreateSupportTicketAsync([FromBody] CreateSupportTicketRequestDto request)
    {
        var command = new CreateSupportTicketCommand(
            request.Title,
            request.Category,
            request.Priority,
            request.InitialMessage,
            request.AttachmentObjectKeys);

        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves paginated support tickets created by the authenticated user.
    /// </summary>
    /// <param name="pageSize">Page size (default 10).</param>
    /// <param name="currentPage">Current page (default 1).</param>
    /// <param name="status">Optional ticket status filter.</param>
    /// <response code="200">List of user tickets.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpGet("my")]
    [Authorize]
    [ProducesResponseType(typeof(Result<Paged<SupportTicketSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result<Paged<SupportTicketSummaryDto>>>> GetMySupportTicketsAsync(
        [FromQuery] int pageSize = 10,
        [FromQuery] int currentPage = 1,
        [FromQuery] TicketStatus? status = null)
    {
        var query = new GetMySupportTicketsQuery(pageSize, currentPage, status);
        return (await _mediator.Send(query)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paginated list of all support tickets with optional filters (requires Support.Tickets.View permission).
    /// </summary>
    /// <param name="pageSize">Page size (default 10).</param>
    /// <param name="currentPage">Current page (default 1).</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="priority">Optional priority filter.</param>
    /// <param name="category">Optional category filter.</param>
    /// <param name="assignedAgentId">Optional assigned agent filter.</param>
    /// <response code="200">List of tickets matching filters.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden if lacking Support.Tickets.View permission.</response>
    [HttpGet]
    [HasPermission(Permissions.Support.TicketsView)]
    [ProducesResponseType(typeof(Result<Paged<SupportTicketSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [RequireSession]
    public async Task<ActionResult<Result<Paged<SupportTicketSummaryDto>>>> GetSupportTicketsAsync(
        [FromQuery] int pageSize = 10,
        [FromQuery] int currentPage = 1,
        [FromQuery] TicketStatus? status = null,
        [FromQuery] TicketPriority? priority = null,
        [FromQuery] SupportTeckitCategory? category = null,
        [FromQuery] Guid? assignedAgentId = null)
    {
        var query = new GetSupportTicketsQuery(pageSize, currentPage, status, priority, category, assignedAgentId);
        return (await _mediator.Send(query)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves detailed information for a specific support ticket including conversation history.
    /// </summary>
    /// <param name="id">Ticket identifier.</param>
    /// <response code="200">Ticket details.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden if not the ticket owner or lacking Support.Tickets.View permission.</response>
    /// <response code="404">Ticket not found.</response>
    [HttpGet("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result<SupportTicketDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result<SupportTicketDetailsDto>>> GetSupportTicketByIdAsync([FromRoute] Guid id)
    {
        var query = new GetSupportTicketByIdQuery(id);
        return (await _mediator.Send(query)).ToActionResult(this);
    }

    /// <summary>
    /// Assigns a support ticket to a designated support agent (requires Support.Tickets.Assign permission).
    /// </summary>
    /// <param name="id">Ticket identifier.</param>
    /// <param name="request">Assignment request.</param>
    /// <response code="200">Ticket assigned successfully.</response>
    /// <response code="400">Invalid agent or ticket state.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden if lacking Support.Tickets.Assign permission.</response>
    /// <response code="404">Ticket not found.</response>
    [HttpPut("{id:guid}/assign")]
    [HasPermission(Permissions.Support.TicketsAssign)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> AssignSupportTicketAsync(
        [FromRoute] Guid id,
        [FromBody] AssignSupportTicketRequestDto request)
    {
        var command = new AssignSupportTicketCommand(id, request.AgentId);
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Updates the priority level of a support ticket (requires Support.Tickets.ChangePriority permission).
    /// </summary>
    /// <param name="id">Ticket identifier.</param>
    /// <param name="request">New priority level.</param>
    /// <response code="200">Priority updated successfully.</response>
    /// <response code="400">Invalid priority value.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden if lacking Support.Tickets.ChangePriority permission.</response>
    /// <response code="404">Ticket not found.</response>
    [HttpPut("{id:guid}/priority")]
    [HasPermission(Permissions.Support.TicketsChangePriority)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> ChangeSupportTicketPriorityAsync(
        [FromRoute] Guid id,
        [FromBody] ChangeSupportTicketPriorityRequestDto request)
    {
        var command = new ChangeSupportTicketPriorityCommand(id, request.Priority);
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Updates the lifecycle status of a support ticket (requires Support.Tickets.ChangeStatus permission).
    /// </summary>
    /// <param name="id">Ticket identifier.</param>
    /// <param name="request">New ticket status.</param>
    /// <response code="200">Status updated successfully.</response>
    /// <response code="400">Invalid status transition.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden if lacking Support.Tickets.ChangeStatus permission.</response>
    /// <response code="404">Ticket not found.</response>
    [HttpPut("{id:guid}/status")]
    [HasPermission(Permissions.Support.TicketsChangeStatus)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> ChangeSupportTicketStatusAsync(
        [FromRoute] Guid id,
        [FromBody] ChangeSupportTicketStatusRequestDto request)
    {
        var command = new ChangeSupportTicketStatusCommand(id, request.Status);
        return (await _mediator.Send(command)).ToActionResult(this);
    }
}
