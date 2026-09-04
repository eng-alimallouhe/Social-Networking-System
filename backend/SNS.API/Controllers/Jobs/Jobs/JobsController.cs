using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Jobs.Jobs.Commands.CloseJob;
using SNS.Application.Jobs.Jobs.Commands.CreateJob;
using SNS.Application.Jobs.Jobs.Commands.DeleteJob;
using SNS.Application.Jobs.Jobs.Commands.UpdateJob;
using SNS.Application.Jobs.Jobs.Contracts;
using SNS.Application.Jobs.Jobs.Queries.GetJobById;
using SNS.Application.Jobs.Jobs.Queries.GetJobsByCompany;
using SNS.Application.Jobs.Jobs.Queries.GetMyCompanyJobs;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.API.Attributes;

namespace SNS.API.Controllers.Jobs.Jobs;

/// <summary>
/// Handles operations for posting, managing, closing, and querying job opportunities.
/// </summary>
[Route("api/v{version:apiVersion}/jobs")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class JobsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new job posting for an active company.
    /// </summary>
    /// <param name="command">Job creation parameters.</param>
    /// <response code="201">Job successfully created.</response>
    /// <response code="400">Invalid parameters or salary range.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user is not a company administrator.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [RequireSession]
    public async Task<ActionResult<Result<Guid>>> CreateJobAsync([FromBody] CreateJobCommand command)
    {
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves full details of a job posting by its ID.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <response code="200">Job details retrieved successfully.</response>
    /// <response code="404">Job not found.</response>
    [HttpGet("{jobId:guid}")]
    [ProducesResponseType(typeof(Result<JobDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<JobDetailsDto>>> GetJobByIdAsync([FromRoute] Guid jobId)
    {
        return (await _mediator.Send(new GetJobByIdQuery(jobId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paginated list of job postings for companies managed by the authenticated user.
    /// </summary>
    /// <param name="companyId">Optional filter for a specific company.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="currentPage">Current page index.</param>
    /// <param name="includeClosed">Whether to include closed jobs in the results.</param>
    /// <response code="200">Company jobs retrieved successfully.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpGet("my-company-jobs")]
    [Authorize]
    [ProducesResponseType(typeof(Result<Paged<JobSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [RequireSession]
    public async Task<ActionResult<Result<Paged<JobSummaryDto>>>> GetMyCompanyJobsAsync(
        [FromQuery] Guid? companyId = null,
        [FromQuery] int pageSize = 10,
        [FromQuery] int currentPage = 1,
        [FromQuery] bool includeClosed = true)
    {
        return (await _mediator.Send(new GetMyCompanyJobsQuery(companyId, pageSize, currentPage, includeClosed))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paginated list of active job postings belonging to a specific company.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="currentPage">Current page index.</param>
    /// <param name="includeClosed">Whether to include closed jobs.</param>
    /// <response code="200">Jobs retrieved successfully.</response>
    [HttpGet("company/{companyId:guid}")]
    [ProducesResponseType(typeof(Result<Paged<JobSummaryDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<Result<Paged<JobSummaryDto>>>> GetJobsByCompanyAsync(
        [FromRoute] Guid companyId,
        [FromQuery] int pageSize = 10,
        [FromQuery] int currentPage = 1,
        [FromQuery] bool includeClosed = false)
    {
        return (await _mediator.Send(new GetJobsByCompanyQuery(companyId, pageSize, currentPage, includeClosed))).ToActionResult(this);
    }

    /// <summary>
    /// Updates details of an existing job posting.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <param name="command">Job update parameters.</param>
    /// <response code="200">Job successfully updated.</response>
    /// <response code="400">Invalid parameters or salary range.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user is not a company administrator.</response>
    /// <response code="404">Job not found.</response>
    [HttpPut("{jobId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> UpdateJobAsync(
        [FromRoute] Guid jobId,
        [FromBody] UpdateJobCommand command)
    {
        var request = command with { JobId = jobId };
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Soft-deletes a job posting.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <response code="200">Job successfully deleted.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user is not a company administrator.</response>
    /// <response code="404">Job not found.</response>
    [HttpDelete("{jobId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> DeleteJobAsync([FromRoute] Guid jobId)
    {
        return (await _mediator.Send(new DeleteJobCommand(jobId))).ToActionResult(this);
    }

    /// <summary>
    /// Closes an active job posting, preventing further applications.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <response code="200">Job successfully closed.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user is not a company administrator.</response>
    /// <response code="404">Job not found.</response>
    [HttpPatch("{jobId:guid}/close")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    [RequireSession]
    public async Task<ActionResult<Result>> CloseJobAsync([FromRoute] Guid jobId)
    {
        return (await _mediator.Send(new CloseJobCommand(jobId))).ToActionResult(this);
    }
}
