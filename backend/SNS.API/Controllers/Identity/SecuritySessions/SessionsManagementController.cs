using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Commands.ForceRevokeUserSessions;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Commands.Logout;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Commands.LogoutFromOtherDevices;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Commands.LogoutFromSession;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetSessionDetails;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetUserActiveSessionsAndDevices;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetUserSessions;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.SecuritySessions
{
    [Route("api/v{version:apiVersion}/identity/security-sessions/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class SessionsManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SessionsManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles="Admin")]
        [HttpPost("force-revoke-user-sessions")]
        public async Task<ActionResult<Result>> ForceRevokeUserSessionsAsync([FromBody] ForceRevokeUserSessionsCommand request)
        {
            return (await _mediator.Send(request)).ToActionResult(this);
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<ActionResult<Result>> LogoutAsync()
        {
            return (await _mediator.Send(new LogoutCommand())).ToActionResult(this);
        }

        [Authorize]
        [HttpPost("logout-from-session")]
        public async Task<ActionResult<Result>> LogoutFromSessionAsync([FromBody] LogOutFromSessionCommand request)
        {
            return (await _mediator.Send(request)).ToActionResult(this);
        }

        [Authorize]
        [HttpPost("logout-from-other-devices")]
        public async Task<ActionResult<Result>> LogoutFromOtherDevicesAsync()
        {
            return (await _mediator.Send(new LogoutFromOtherDevicesCommand())).ToActionResult(this);
        }

        [Authorize]
        [HttpGet("sessions-details/{sessionId:guid}")]
        public async Task<ActionResult<Result<SessionDetaildDto>>> GetSessionDeatilsAsync([FromRoute] Guid sessionId)
        {
            return (await _mediator.Send(new GetSessionDetailsQuery(sessionId))).ToActionResult(this);
        }

        [Authorize]
        [HttpGet("user-active-sessions-and-devices")]
        public async Task<ActionResult<Result<UserActiveSessionsAndDevicesResult>>> GetUserActiveSessionsAndDevicesAsync()
        {
            return (await _mediator.Send(new GetUserActiveSessionsAndDevicesQuery())).ToActionResult(this);
        }

        [Authorize]
        [HttpGet("user-sessions/{targetUserId:guid}")]
        public async Task<ActionResult<Result<Paged<SessionSummaryDto>>>> GetUserSessionAsync([FromRoute] GetUserSessionsQuery request)
        {
            return (await _mediator.Send(request)).ToActionResult(this);
        }
    }
}