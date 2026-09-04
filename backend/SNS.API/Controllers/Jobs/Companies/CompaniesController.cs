using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Jobs.Companies.Commands.DeleteCompany;
using SNS.Application.Jobs.Companies.Commands.UpdateCompany;
using SNS.Application.Jobs.Companies.Contracts;
using SNS.Application.Jobs.Companies.Queries.GetCompanyById;
using SNS.Application.Jobs.Companies.Queries.GetMyCompanies;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Jobs.Companies;

/// <summary>
/// Handles querying, updating, and deleting companies.
/// </summary>
[Route("api/v{version:apiVersion}/companies")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class CompaniesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CompaniesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Retrieves full details of a company by its ID.
    /// </summary>
    /// <param name="companyId">The company unique identifier.</param>
    /// <response code="200">Company details retrieved successfully.</response>
    /// <response code="404">Company not found.</response>
    [HttpGet("{companyId:guid}")]
    [ProducesResponseType(typeof(Result<CompanyDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<CompanyDetailsDto>>> GetCompanyByIdAsync([FromRoute] Guid companyId)
    {
        return (await _mediator.Send(new GetCompanyByIdQuery(companyId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves all companies managed by the authenticated user.
    /// </summary>
    /// <response code="200">User's companies retrieved successfully.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpGet("my-companies")]
    [Authorize]
    [ProducesResponseType(typeof(Result<List<CompanySummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result<List<CompanySummaryDto>>>> GetMyCompaniesAsync()
    {
        return (await _mediator.Send(new GetMyCompaniesQuery())).ToActionResult(this);
    }

    /// <summary>
    /// Updates details of an existing company.
    /// </summary>
    /// <param name="companyId">The company unique identifier.</param>
    /// <param name="command">Company update parameters.</param>
    /// <response code="200">Company successfully updated.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user is not a company administrator.</response>
    /// <response code="404">Company not found.</response>
    [HttpPut("{companyId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> UpdateCompanyAsync(
        [FromRoute] Guid companyId,
        [FromBody] UpdateCompanyCommand command)
    {
        var request = command with { CompanyId = companyId };
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Soft-deletes an existing company.
    /// </summary>
    /// <param name="companyId">The company unique identifier.</param>
    /// <response code="200">Company successfully deleted.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user is not the company owner.</response>
    /// <response code="404">Company not found.</response>
    [HttpDelete("{companyId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> DeleteCompanyAsync([FromRoute] Guid companyId)
    {
        return (await _mediator.Send(new DeleteCompanyCommand(companyId))).ToActionResult(this);
    }
}
