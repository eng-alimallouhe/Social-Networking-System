using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Identity.ArchiveManagement.Commands.ExportAccountData;
using SNS.Application.Identity.ArchiveManagement.Contracts;
using SNS.Application.Identity.ArchiveManagement.Qureies.GetUserArchive;
using SNS.Application.Identity.ArchiveManagement.Qureies.GetUserIdentityArchive;
using SNS.Application.Identity.ArchiveManagement.Qureies.GetUserPasswordArchive;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.ArchiveManagement;

[Route("api/v{version:apiVersion}/identity/[controller]")]
[ApiVersion("1.0")]
[ApiController]
public class ArchiveManagementController : ControllerBase
{
    private readonly IMediator _mediator;

    public ArchiveManagementController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [HttpGet("user-archive")]
    public async Task<ActionResult<Result<Paged<UserArchiveSummaryDto>>>> GetUserArchiveAsync([FromBody] GetUserArchiveQuery request)
    {
        var result = await _mediator.Send(request);
        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpGet("user-identity-archive")]
    public async Task<ActionResult<Result<Paged<UserArchiveSummaryDto>>>> GetUserIdentityArchiveAsync([FromBody] GetUserIdentityArchiveQuery request)
    {
        var result = await _mediator.Send(request);
        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpGet("user-password-archive")]
    public async Task<ActionResult<Result<Paged<UserPasswordArchiveSummaryDto>>>> GetUserIdentityArchiveAsync([FromBody] GetUserPasswordArchiveQuery request)
    {
        var result = await _mediator.Send(request);
        return result.ToActionResult(this);
    }

    [Authorize]
    [HttpPost("export-account-data")]
    public async Task<ActionResult<Result<ExportAccountDataResponseDto>>> ExportAccountDataAsync([FromBody] ExportAccountDataCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }
}
