using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Profiles.Profiles.Commands.ViewProfiles;

public sealed record ViewProfilesCommand(
    List<Guid> ViewedProfileIds): ICommand;
