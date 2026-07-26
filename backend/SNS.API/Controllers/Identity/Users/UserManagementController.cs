using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Queries.GetUserAccount;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Users.UsersManagement.Commands.BeginUserDeactivation;
using SNS.Application.Identity.Users.UsersManagement.Commands.CancelUserDeactivationRequest;
using SNS.Application.Identity.Users.UsersManagement.Commands.ChangeUserName;
using SNS.Application.Identity.Users.UsersManagement.Commands.ChangeUserPreferredLanguage;
using SNS.Application.Identity.Users.UsersManagement.Commands.CompleteUserDeactivation;
using SNS.Application.Identity.Users.UsersManagement.Queries.checkUsernameAvailabilty;
using SNS.Application.Identity.Users.UsersManagement.Queries.GetUserInformation;
using SNS.Application.Identity.Users.UsersManagement.Queries.GetUserSecurityDetails;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.Users
{
    [Route("api/v{version:apiVersion}/identity/users/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class UserManagementController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserManagementController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize]
        [HttpPost("begin-user-deactivation")]
        public async Task<ActionResult<Result<BeginUserDeactivationResponse>>> BeginUserDeactivationAsync([FromBody] BeginUserDeactivationCommand request)
        {
            return (await _mediator.Send(request)).ToActionResult(this);
        }

        [Authorize]
        [HttpPost("cancel-user-deactivation")]
        public async Task<ActionResult<AuthTokensDto>> CancelUserDeactivationAsync([FromBody] CancelUserDeactivationRequestCommand request)
        {
            return (await _mediator.Send(request)).ToActionResult(this);
        }

        [Authorize]
        [HttpPost("complete-user-deactivation")]
        public async Task<ActionResult<Result>> CompleteUserDeactivationAsync([FromBody] CompleteUserDeactivationCommand request)
        {
            return (await _mediator.Send(request)).ToActionResult(this);
        }


        [Authorize]
        [HttpPut("update-username")]
        public async Task<ActionResult> UpdateUserNameAsync([FromBody] ChangeUserNameCommand request)
        {
            return (await _mediator.Send(request)).ToActionResult(this);
        }

        [Authorize]
        [HttpPost("update-user-preferred-language")]
        public async Task<ActionResult> UpdateUserPreferredLangaugeAsync([FromBody] ChangeUserPreferredLanguageCommand request)
        {
            return (await _mediator.Send(request)).ToActionResult(this);
        }


        [Authorize]
        [HttpGet("user-information")]
        public async Task<ActionResult<Result<UserInformationResult?>>> GetUserInformationAsync()
        {
            return (await _mediator.Send(new GetUserInformationQuery())).ToActionResult(this);
        }

        [Authorize]
        [HttpGet("user-security-details")]
        public async Task<ActionResult<Result<UserSecurityDetailsResult?>>> GetUserSecurityDetailsAsync()
        {
            return (await _mediator.Send(new GetUserSecurityDetailsQuery())).ToActionResult(this);
        }

        [MapToApiVersion("1.0")]
        [Authorize]
        [HttpGet("user-account")]
        public async Task<ActionResult<Result<UserAccount>>> GetUserAccountAsync()
        {
            return (await _mediator.Send(new GetUserAccountQuery())).ToActionResult(this);
        }

        [MapToApiVersion("1.0")]
        [Authorize]
        [HttpGet("username-available")]
        public async Task<ActionResult<Result<bool>>> CheckUsernameAvailabilityAsync([FromQuery] string username)
        {
            return (await _mediator.Send(new CheckUsernameAvailabiltyQuery(username))).ToActionResult(this);
        }

    }
}
