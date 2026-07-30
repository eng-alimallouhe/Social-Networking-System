using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Authentication;


namespace SNS.Application.Identity.SecuritySettings.Recovery.Commands.RecoverAccountBySecurityCode;

/// <summary>
/// Represents a command to recover access to a user account using a security code.
/// </summary>
/// <param name="SecurityCode">The security code provided by the user for account recovery.</param>
public sealed record RecoverAccountBySecurityCodeCommand(string SecurityCode) : ICommand<AuthTokensDto>;

