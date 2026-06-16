namespace SNS.Application.Identity.Shared.DTOs.Authentication;

/// <summary>
/// Represents a data transfer object used to
/// return authentication credentials after a successful login or registration.
/// </summary>
/// <param name="Token">Gets the JWT Access Token. This value is used to authenticate subsequent API requests via the Authorization header.</param>
/// <param name="RefreshToken">Gets the Refresh Token. This value is used to obtain a new Access Token when the current one expires.</param>
public sealed record AuthTokensDto(
    string Token,
    string RefreshToken);
