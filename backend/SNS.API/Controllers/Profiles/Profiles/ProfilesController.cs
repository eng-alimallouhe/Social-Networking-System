using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.DTOs.Profiles;
using SNS.API.Extensions;
using SNS.Application.Profiles.Profiles.Commands.CreateProfile;
using SNS.Application.Profiles.Profiles.Commands.UpdateBasicInformation;
using SNS.Application.Profiles.Profiles.Commands.UpdateProfilePicture;
using SNS.Application.Profiles.Profiles.Commands.UpdateSocialLinks;
using SNS.Application.Profiles.Profiles.Contracts;
using SNS.Application.Profiles.Profiles.Queries.GetBasicInformation;
using SNS.Application.Profiles.Profiles.Queries.GetProfileById;
using SNS.Application.Profiles.Profiles.Queries.GetProfileForCurrentUser;
using SNS.Application.Profiles.Profiles.Queries.GetProfilePictureUrl;
using SNS.Application.Profiles.Profiles.Queries.GetSocialLinks;
using SNS.Application.Shared.Contracts.Storage;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Profiles.Profiles;

[Route("api/v{version:apiVersion}/profiles/[controller]")]
[ApiVersion("1.0")]
[ApiController]
public class ProfilesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfilesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("base")]
    public async Task<ActionResult<Result<ProfileBaseDto>>> GetProfileForCurrentUserAsync()
    {
        return (await _mediator.Send(new GetProfileForCurrentUserQuery())).ToActionResult(this);
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPost()]
    public async Task<ActionResult<Result>> CreateProfileAsync([FromForm] CreateProfileRequest request)
    {
        UploadedFile? uploadedFile = null;

        if (request.ProfilePicture is not null)
        {
            uploadedFile = new UploadedFile(
                Stream: request.ProfilePicture.OpenReadStream(),
                FileName: request.ProfilePicture.FileName,
                ContentType: request.ProfilePicture.ContentType,
                Extension: Path.GetExtension(request.ProfilePicture.FileName)
                                .TrimStart('.')
                                .ToLowerInvariant(),
                Length: request.ProfilePicture.Length
            );
        }
        return (await _mediator.Send(new CreateProfileCommand(
            FullName: request.FullName,
            Specialization: request.Specialization,
            Bio: request.Bio,
            ProfilePicture: uploadedFile
        ))).ToActionResult(this);
    }


    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("basic-information")]
    public async Task<ActionResult<Result>> UpdateBasicInformationAsync([FromBody] UpdateBasicInformationCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }


    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("profile-picture")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<Result>> UpdateProfilePictureAsync(IFormFile profilePicture)
    {
        return (await _mediator.Send(new UpdateProfilePictureCommand(profilePicture.ToUploadedFile()))).ToActionResult(this);
    }


    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpPut("social-links")]
    public async Task<ActionResult<Result>> UpdateSocialLinksAsync([FromBody] UpdateSocialLinksCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("basic-information")]
    public async Task<ActionResult<Result<ProfileBaseDto>>> GetBasicInformationAsync()
    {
        return (await _mediator.Send(new GetBasicInformationQuery())).ToActionResult(this);
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("profile-picture")]
    public async Task<ActionResult<Result<string>>> GetProfilePictureAsync()
    {
        return (await _mediator.Send(new GetProfilePictureUrlQuery())).ToActionResult(this);
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet("social-links")]
    public async Task<ActionResult<Result<SocialLinksDto>>> GetSocialLinksAsync()
    {
        return (await _mediator.Send(new GetSocialLinksQuery())).ToActionResult(this);
    }

    [Authorize]
    [MapToApiVersion("1.0")]
    [HttpGet(":id{guid}")]
    public async Task<ActionResult<Result<ProfileDetailsDto>>> GetProfileAsync(Guid id)
    {
        return (await _mediator.Send(new GetProfileByIdQuery(id))).ToActionResult(this);
    }
}
