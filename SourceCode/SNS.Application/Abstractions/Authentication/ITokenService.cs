using SNS.Application.DTOs.Authentication.Responses;
using SNS.Common.Results;
using SNS.Domain.Security.Entities;

namespace SNS.Application.Abstractions.Authentication;

public interface ITokenService
{
    /// <summary>
    /// Generates a JWT Access Token including User Claims and Session ID.
    /// </summary>
    string GenerateAccessToken(User user, Guid sessionId);

    /// <summary>
    /// Generates a cryptographically secure random string for Refresh Token.
    /// </summary>
    Task<string> GrantRefreshTokenAsync(User user);

    /// <summary>
    /// Validates the old tokens, rotates the refresh token, creates a NEW session,
    /// and returns a fresh pair of tokens.
    /// </summary>
    Task<Result<AuthenticationResultDto>> RefreshTokensAsync(string oldAccessToken, string incomingRefreshToken);
}
