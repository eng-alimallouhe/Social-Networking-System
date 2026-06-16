using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.DTOs.Authentication;


namespace SNS.Application.Identity.SecuritySettings.Recovery.Commands.RecoverAccountBySecurityCode;

public sealed record RecoverAccountBySecurityCodeCommand(string SecurityCode) : ICommand<AuthTokensDto>;
