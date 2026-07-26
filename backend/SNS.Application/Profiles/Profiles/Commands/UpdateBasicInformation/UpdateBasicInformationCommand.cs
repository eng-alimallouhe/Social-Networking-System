using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Profiles.Profiles.Commands.UpdateBasicInformation;

public sealed record UpdateBasicInformationCommand(
    string FullName,
    string Bio, 
    string Specialization,
    string Location
) : ICommand;
