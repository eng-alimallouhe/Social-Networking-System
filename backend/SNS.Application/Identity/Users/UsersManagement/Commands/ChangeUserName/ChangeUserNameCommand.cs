using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Identity.Users.UsersManagement.Commands.ChangeUserName;

public sealed record ChangeUserNameCommand(string NewUserName) : ICommand;
