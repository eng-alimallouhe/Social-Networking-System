using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SNS.API.Extensions;
using SNS.Application.Abstractions.Authentication;
using SNS.Application.DTOs.Authentication.Account.Requests;
using SNS.Application.DTOs.Authentication.Account.Responses;
using SNS.Application.DTOs.Authentication.Common.Responses;
using SNS.Application.DTOs.Authentication.Login.Requests;
using SNS.Application.DTOs.Authentication.Password.Requests;
using SNS.Application.DTOs.Authentication.TwoFactor.Requests;
using SNS.Common.Results;
using System.Security.Claims;

namespace SNS.API.Controllers.Authentication;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    // ------------------------------------------------------------------
    // Authentication & Session Management
    // ------------------------------------------------------------------

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<AuthTokensDto>>> Login([FromBody] LoginDto loginDto)
    {
        var result = await _accountService.LoginAsync(loginDto);
        return result.ToActionResult(this);
    }

    [HttpPost("logout")]
    [Authorize]
    // هنا تركناها ActionResult فقط، لأن الـ Extension للـ Result العادية 
    // بترجع anonymous object: new { message = "Success" } وليس Result object
    public async Task<ActionResult<Result>> Logout(
        [FromBody] string refreshToken)
    {
        var result = await _accountService.LogoutAsync(refreshToken);
        return result.ToActionResult(this);
    }

    // ------------------------------------------------------------------
    // Password Management
    // ------------------------------------------------------------------

    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<Result<AuthTokensDto>>> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var result = await _accountService.ChangePasswordAsync(dto);
        return result.ToActionResult(this);
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<ActionResult<Result>> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        var result = await _accountService.ForgotPasswordAsync(dto);
        return result.ToActionResult(this);
    }

    [HttpPost("verify-reset-code")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<Guid>>> VerifyResetCode([FromBody] VerifyResetCodeDto dto)
    {
        var result = await _accountService.VerifyResetCodeAsync(dto);
        return result.ToActionResult(this);
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<AuthTokensDto>>> ResetPassword([FromBody] ResetPasswordDto dto)
    {
        var result = await _accountService.ResetPasswordAsync(dto);
        return result.ToActionResult(this);
    }

    // ------------------------------------------------------------------
    // Two-Factor Authentication (2FA)
    // ------------------------------------------------------------------

    [HttpPost("2fa/validate")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<AuthTokensDto>>> ValidateTwoFactor([FromBody] TwoFactorVerificationDto dto)
    {
        var result = await _accountService.ValidateTwoFactorCodeAsync(dto);
        return result.ToActionResult(this);
    }

    [HttpPost("2fa/resend")]
    [AllowAnonymous]
    public async Task<ActionResult<Result>> ResendTwoFactor([FromBody] string userIdentifier)
    {
        var result = await _accountService.ResendTwoFactorCodeAsync(userIdentifier);
        return result.ToActionResult(this);
    }

    // ------------------------------------------------------------------
    // Identifier Changes (Email/Phone Update)
    // ------------------------------------------------------------------

    [HttpPost("identifier/change/initiate")]
    [Authorize]
    public async Task<ActionResult<Result<InitiateIdentifierChangeResultDto>>> InitiateIdentifierChange([FromBody] InitiateIdentifierChangeDto dto)
    {
        var result = await _accountService.InitiateIdentifierChangeAsync(dto);
        return result.ToActionResult(this);
    }

    [HttpPost("identifier/change/verify")]
    [Authorize]
    public async Task<ActionResult<Result<AuthTokensDto>>> VerifyIdentifierChange([FromBody] VerifyIdentifierChangeDto dto)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdString, out Guid userId))
        {
            return Unauthorized();
        }

        var result = await _accountService.VerifyIdentifierChangeAsync(userId, dto);
        return result.ToActionResult(this);
    }

    // ------------------------------------------------------------------
    // Support & Admin Recovery Actions
    // ------------------------------------------------------------------

    [HttpPost("support/phone/initiate")]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult<Result<string>>> InitiateSupportPhoneChange([FromBody] SupportResetPhoneNumberDto dto)
    {
        var result = await _accountService.InitiateSupportPhoneChangeAsync(dto);
        return result.ToActionResult(this);
    }

    [HttpPost("support/code/resend")]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult<Result>> ResendSupportCode([FromBody] ResendSupportCodeDto dto)
    {
        var result = await _accountService.ResendSupportCodeAsync(dto);
        return result.ToActionResult(this);
    }

    [HttpPost("support/phone/verify")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<AuthTokensDto>>> VerifySupportPhoneChange([FromBody] VerifySupportPhoneChangeDto dto)
    {
        var result = await _accountService.VerifySupportPhoneChangeAsync(dto);
        return result.ToActionResult(this);
    }

    [HttpPost("recovery/security-code")]
    [AllowAnonymous]
    public async Task<ActionResult<Result<AuthTokensDto>>> RecoveryAccountBySecurityCode([FromBody] RecoveryAccountBySecurityCodeDto dto)
    {
        var result = await _accountService.RecoveryAccountBySecurityCodeAsync(dto);
        return result.ToActionResult(this);
    }
}