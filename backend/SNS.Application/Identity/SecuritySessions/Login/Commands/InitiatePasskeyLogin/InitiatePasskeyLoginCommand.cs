using Fido2NetLib;
using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.SecuritySessions.Login.Commands.InitiatePasskeyLogin;

public sealed record InitiatePasskeyLoginCommand(string Identifier) : ICommand<AssertionOptions>;
