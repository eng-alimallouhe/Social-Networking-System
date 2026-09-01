using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Jobs.JobApplications.Commands.CreateJobApplication;
using SNS.Application.Jobs.JobApplications.Commands.UpdateJobApplicationStatus;
using SNS.Application.Jobs.JobApplications.Commands.WithdrawJobApplication;
using SNS.Application.Jobs.JobApplications.Contracts;
using SNS.Application.Jobs.JobApplications.Queries.GetJobApplicationById;
using SNS.Application.Jobs.JobApplications.Queries.GetJobApplications;
using SNS.Application.Jobs.JobApplications.Queries.GetMyJobApplications;
using SNS.Application.Shared.DTOs;
using SNS.Domain.QA.Enums;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Jobs.JobApplications;

/// <summary>
/// Request payload for updating the status of a job application.
/// </summary>
/// <param name="NewStatus">The updated application status.</param>
public sealed record UpdateJobApplicationStatusRequest(ApplicationStatus NewStatus);

/// <summary>
/// Handles applying for jobs, applicant withdrawal, reviewing submitted applications, and updating applicant statuses.
/// </summary>
[Route("api/v{version:apiVersion}/job-applications")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class JobApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public JobApplicationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Submits a new job application.
    /// </summary>
    /// <param name="command">Job application details.</param>
    /// <response code="201">Application successfully submitted.</response>
    /// <response code="400">Invalid parameters, duplicate application, or job is closed.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<Guid>>> CreateJobApplicationAsync([FromBody] CreateJobApplicationCommand command)
    {
        return (await _mediator.Send(command)).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves full details of a specific job application.
    /// </summary>
    /// <param name="applicationId">The unique identifier of the application.</param>
    /// <response code="200">Application details retrieved successfully.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user is neither the applicant nor a company administrator.</response>
    /// <response code="404">Application not found.</response>
    [HttpGet("{applicationId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result<JobApplicationDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<JobApplicationDetailsDto>>> GetJobApplicationByIdAsync([FromRoute] Guid applicationId)
    {
        return (await _mediator.Send(new GetJobApplicationByIdQuery(applicationId))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paginated list of job applications submitted by the authenticated user.
    /// </summary>
    /// <param name="status">Optional filter by application status.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="currentPage">Current page index.</param>
    /// <response code="200">Applications retrieved successfully.</response>
    /// <response code="401">Unauthorized.</response>
    [HttpGet("my-applications")]
    [Authorize]
    [ProducesResponseType(typeof(Result<Paged<JobApplicationSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Result<Paged<JobApplicationSummaryDto>>>> GetMyJobApplicationsAsync(
        [FromQuery] ApplicationStatus? status = null,
        [FromQuery] int pageSize = 10,
        [FromQuery] int currentPage = 1)
    {
        return (await _mediator.Send(new GetMyJobApplicationsQuery(status, pageSize, currentPage))).ToActionResult(this);
    }

    /// <summary>
    /// Retrieves a paginated list of applications received for a specific job posting.
    /// </summary>
    /// <param name="jobId">The unique identifier of the job.</param>
    /// <param name="companyId">Optional company identifier filter.</param>
    /// <param name="status">Optional filter by application status.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <param name="currentPage">Current page index.</param>
    /// <response code="200">Applications retrieved successfully.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user is not a company administrator.</response>
    [HttpGet("job/{jobId:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(Result<Paged<JobApplicationSummaryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<Result<Paged<JobApplicationSummaryDto>>>> GetJobApplicationsAsync(
        [FromRoute] Guid jobId,
        [FromQuery] Guid? companyId = null,
        [FromQuery] ApplicationStatus? status = null,
        [FromQuery] int pageSize = 10,
        [FromQuery] int currentPage = 1)
    {
        return (await _mediator.Send(new GetJobApplicationsQuery(jobId, companyId, status, pageSize, currentPage))).ToActionResult(this);
    }

    /// <summary>
    /// Withdraws a job application previously submitted by the authenticated user.
    /// </summary>
    /// <param name="applicationId">The unique identifier of the application.</param>
    /// <response code="200">Application successfully withdrawn.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user is not the applicant.</response>
    /// <response code="404">Application not found.</response>
    [HttpPatch("{applicationId:guid}/withdraw")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> WithdrawJobApplicationAsync([FromRoute] Guid applicationId)
    {
        return (await _mediator.Send(new WithdrawJobApplicationCommand(applicationId))).ToActionResult(this);
    }

    /// <summary>
    /// Updates the review status of a candidate's job application.
    /// </summary>
    /// <param name="applicationId">The unique identifier of the application.</param>
    /// <param name="request">New application status payload.</param>
    /// <response code="200">Status successfully updated.</response>
    /// <response code="400">Application has already been withdrawn.</response>
    /// <response code="401">Unauthorized.</response>
    /// <response code="403">Current user is not a company administrator.</response>
    /// <response code="404">Application or job not found.</response>
    [HttpPatch("{applicationId:guid}/status")]
    [Authorize]
    [ProducesResponseType(typeof(Result), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(Result), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result>> UpdateJobApplicationStatusAsync(
        [FromRoute] Guid applicationId,
        [FromBody] UpdateJobApplicationStatusRequest request)
    {
        var command = new UpdateJobApplicationStatusCommand(applicationId, request.NewStatus);
        return (await _mediator.Send(command)).ToActionResult(this);
    }
}
