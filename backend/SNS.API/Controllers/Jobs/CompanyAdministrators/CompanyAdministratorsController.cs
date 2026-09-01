using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Jobs.CompanyAdministrators.Commands.AddCompanyAdministrator;
using SNS.Application.Jobs.CompanyAdministrators.Commands.ChangeCompanyAdministratorRole;
using SNS.Application.Jobs.CompanyAdministrators.Commands.RemoveCompanyAdministrator;
using SNS.Application.Jobs.CompanyAdministrators.Contracts;
using SNS.Application.Jobs.CompanyAdministrators.Queries.GetCompanyAdministrators;
using SNS.Application.Jobs.CompanyAdministrators.Queries.GetMyCompanyAdministratorRole;
using SNS.Domain.Jobs.Enums;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Jobs.CompanyAdministrators;

/// <summary>
/// Request payload for adding a new company administrator.
/// </summary>
/// <param name="TargetProfileId">The profile ID of the user to designate as administrator.</param>
/// <param name="Role">The administrative role (Owner or Manager).</param>
public sealed record AddCompanyAdministratorRequest(Guid TargetProfileId, CompanyRole Role = CompanyRole.Manager);

/// <summary>
/// Request payload for changing the role of an existing company administrator.
/// </summary>
/// <param name="NewRole">The new administrative role (Owner or Manager).</param>
public sealed record ChangeCompanyAdministratorRoleRequest(CompanyRole NewRole);

/// <summary>
/// Handles managing company administrators, invitations, role modifications, and role inquiries.
/// </summary>
[Route("api/v{version:apiVersion}/companies/{companyId:guid}/administrators")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class CompanyAdministratorsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompanyAdministratorsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves all administrators for a specific company.
    /// </summary>
    /// <param name="companyId">The company unique identifier.</param>
    /// <response code="200">Administrators retrieved successfully.</response>
    /// <response code="404">Company not found.</response>
    [HttpGet]
    [ProducesResponseType(typeof(Result<List<CompanyAdministratorDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<List<CompanyAdministratorDto>>>> GetCompanyAdministratorsAsync([FromRoute] Guid companyId)
    {
        return (await _mediator.Send(new GetCompanyAdministratorsQuery(companyId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves the current authenticated user's administrator role for a specific company.
    /// </summary>
    /// <param name="companyId">The company unique identifier.</param>
    /// <response code="200">Role retrieved successfully (null if not an administrator).</response>
    /// <response code="401">Unauthorized.</response>
    [HttpGet("my-role")]
    [Authorize]
    [ProducesResponseType(typeof(Result<CompanyRole?>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<CompanyRole?>>> GetMyCompanyAdministratorRoleAsync([FromRoute] Guid companyId)
    {
        return (await _mediator.Send(new GetMyCompanyAdministratorRoleQuery(companyId))).ToActionResult(this);
    }

    /// <summary>
    /// Adds a new administrator to a company.
    /// </summary>
    /// <param name="companyId">The company unique identifier.</param>
    /// <param name="request">Administrator assignment payload.</param>
    /// <response code="201">Administrator successfully added.</response>
    /// <response code="400">Target profile is already an administrator.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user is not the company owner.</response>
    /// <response code="404">Company or target profile not found.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<Guid>>> AddCompanyAdministratorAsync(
        [FromRoute] Guid companyId,
        [FromBody] AddCompanyAdministratorRequest request)
    {
        var command = new AddCompanyAdministratorCommand(companyId, request.TargetProfileId, request.Role);
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Removes an administrator from a company.
    /// </summary>
    /// <param name="companyId">The company unique identifier.</param>
    /// <param name="targetProfileId">The profile ID of the administrator to remove.</param>
    /// <response code="200">Administrator successfully removed.</response>
    /// <response code="400">Cannot remove the sole company owner.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user is not authorized to remove this administrator.</response>
    /// <response code="404">Administrator record not found.</response>
    [HttpDelete("{targetProfileId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> RemoveCompanyAdministratorAsync(
        [FromRoute] Guid companyId,
        [FromRoute] Guid targetProfileId)
    {
        return (await _mediator.Send(new RemoveCompanyAdministratorCommand(companyId, targetProfileId))).ToActionResult(this);
    }

    /// <summary>
    /// Updates the administrative role of a company administrator.
    /// </summary>
    /// <param name="companyId">The company unique identifier.</param>
    /// <param name="targetProfileId">The profile ID of the administrator.</param>
    /// <param name="request">New role payload.</param>
    /// <response code="200">Role successfully updated.</response>
    /// <response code="400">Cannot demote the sole company owner.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user is not the company owner.</response>
    /// <response code="404">Administrator record not found.</response>
    [HttpPatch("{targetProfileId:guid}/role")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> ChangeCompanyAdministratorRoleAsync(
        [FromRoute] Guid companyId,
        [FromRoute] Guid targetProfileId,
        [FromBody] ChangeCompanyAdministratorRoleRequest request)
    {
        var command = new ChangeCompanyAdministratorRoleCommand(companyId, targetProfileId, request.NewRole);
        return (await _mediator.Send(command)).ToActionResult(this);
    }
}
