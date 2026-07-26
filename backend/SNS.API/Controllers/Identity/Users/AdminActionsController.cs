using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.Users.AdminAcions.Commands.ChangeUserRole;
using SNS.Application.Identity.Users.AdminAcions.Commands.PermanentlyBanUser;
using SNS.Application.Identity.Users.AdminAcions.Commands.UnbanUser;
using SNS.Application.Identity.Users.AdminAcions.Queries.GetUserActivityAnalytics;
using SNS.Application.Identity.Users.AdminAcions.Queries.GetUserDetails;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.Users;

[Route("api/identity/users/[controller]")]
[ApiController]
public class AdminActionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminActionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("change-user-role")]
    public async Task<ActionResult<Result>> ChangeUserRoleAsync([FromBody] ChangeUserRoleCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpPost("permanently-ban-user")]
    public async Task<ActionResult<Result>> PermanentlyBanUserAsync([FromBody] PermanentlyBanUserCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpPost("unban-user")]
    public async Task<ActionResult<Result>> UnbanUserAsync([FromBody] UnbanUserCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpGet("user-activity-analytics")]
    public async Task<ActionResult<Result<UserActivityAnalyticsResult>>> GetUserActivityAnalyticsAsync([FromBody] GetUserActivityAnalyticsQuery request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [HttpGet("user-details")]
    public async Task<ActionResult<Result<UserDetailsDto>>> GetUserDetailsAsync([FromBody] GetUserDetailsQuery request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }
}
