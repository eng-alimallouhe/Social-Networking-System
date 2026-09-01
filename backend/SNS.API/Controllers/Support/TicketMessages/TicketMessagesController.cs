using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Support.TicketMessages.Commands.ReplyToSupportTicket;
using SNS.Application.Support.TicketMessages.Contracts;
using SNS.Application.Support.TicketMessages.Queries.GetTicketMessages;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Support.TicketMessages;

/// <summary>
/// Handles operations for viewing message history and adding replies to support tickets.
/// </summary>
[Route("api/v{version:apiVersion}/support/tickets/{ticketId:guid}/messages")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class TicketMessagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketMessagesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Adds a user or agent reply to an existing open/pending support ticket.
    /// </summary>
    /// <param name="ticketId">Ticket identifier.</param>
    /// <param name="request">Reply details including body and optional attachment keys.</param>
    /// <response code="201">Reply posted successfully.</response>
    /// <response code="400">Ticket is closed or message is invalid.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden if not the ticket owner or lacking Support.Tickets.Reply permission.</response>
    /// <response code="404">Ticket not found.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> ReplyToTicketAsync(
        [FromRoute] Guid ticketId,
        [FromBody] ReplyToSupportTicketRequestDto request)
    {
        var command = new ReplyToSupportTicketCommand(
            ticketId,
            request.MessageBody,
            request.AttachmentObjectKeys);

        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves all conversation messages and attached files for a support ticket.
    /// </summary>
    /// <param name="ticketId">Ticket identifier.</param>
    /// <response code="200">List of ticket messages.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Forbidden if not the ticket owner or lacking Support.Tickets.View permission.</response>
    /// <response code="404">Ticket not found.</response>
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(Result<IReadOnlyList<TicketMessageDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<IReadOnlyList<TicketMessageDto>>>> GetTicketMessagesAsync([FromRoute] Guid ticketId)
    {
        var query = new GetTicketMessagesQuery(ticketId);
        return (await _mediator.Send(query)).ToActionResult(this);
    }
}
