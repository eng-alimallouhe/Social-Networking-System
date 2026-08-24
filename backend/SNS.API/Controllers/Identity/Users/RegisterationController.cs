using Asp.Versioning;
using CloudinaryDotNet;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.API.Helpers;
using SNS.Application.Identity.Users.Registeration.Commands.RegisterUser;
using SNS.Application.Identity.Users.Registeration.Commands.ResendVerifyCode;
using SNS.Application.Identity.Users.Registeration.Commands.VerifyUser;
using SNS.Application.Identity.Users.Registeration.DTOs;
using SNS.Shared.Results;

namespace SNS.API.Controllers.Identity.Users;

/// <summary>
/// Handles user account registration, verification code dispatch, and email verification completion.
/// </summary>
[Route("api/v{version:apiVersion}/identity/users/[controller]")]
[ApiVersion("1.0")]
[ApiController]
[Produces("application/json")]
public class RegisterationController : ControllerBase
{
    private readonly IMediator _mediator;

    public RegisterationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registers a new user account on the platform.
    /// </summary>
    /// <remarks>
    /// Validates registration details, hashes password, creates user entity, and sends email verification code.
    /// </remarks>
    /// <param name="request">The registration request payload containing email, username, and password.</param>
    /// <response code="200">Returns registration result and user ID <see cref="RegisterResponseDto"/>.</response>
    /// <response code="400">The registration details are invalid.</response>
    /// <response code="409">The email address or username is already registered.</response>
    [HttpPost]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Result<RegisterResponseDto>>> RegisterAsync([FromBody] RegisterUserCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Resends the email verification code for an unverified user account.
    /// </summary>
    /// <param name="request">The resend request payload containing user ID or email.</param>
    /// <response code="200">Verification code resent successfully <see cref="RegisterResponseDto"/>.</response>
    /// <response code="400">Account is already verified or request parameters are invalid.</response>
    /// <response code="404">No matching user account was found.</response>
    [HttpPost("resend-verify-code")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Result<RegisterResponseDto>>> ResendVerifyCodeAsync([FromBody] ResendVerifyCodeCommand request)
    {
        return (await _mediator.Send(request)).ToActionResult(this);
    }

    /// <summary>
    /// Verifies the OTP code to activate the newly registered user account.
    /// </summary>
    /// <param name="request">The verification payload containing OTP code and user ID.</param>
    /// <response code="200">User account verified successfully <see cref="RegisterResponseDto"/>.</response>
    /// <response code="400">The verification code is invalid or expired.</response>
    [HttpPost("verify-user")]
    [MapToApiVersion("1.0")]
    [Consumes("application/json")]
    [ProducesResponseType(typeof(RegisterResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Result<AuthToken>>> VerifyUserAsync([FromBody] VerifyUserCommand request)
    {
        var result = await _mediator.Send(request);
        if (result.IsSuccess)
        {
            Response.Cookies.Append(
                CookieFactory.RefreshTokenCookieName,
                result.Value?.RefreshToken ?? string.Empty,
                CookieFactory.CreateRefreshTokenCookie(true));
        }

        return result.ToActionResult(this);
    }
}