using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Profiles.Profiles.Commands.ViewProfiles;

/// <summary>
/// Represents a batch command to record multiple profile view events for the authenticated user.
/// </summary>
/// <param name="ViewedProfileIds">The list of unique identifiers for profiles that were viewed.</param>
public sealed record ViewProfilesCommand(
    List<Guid> ViewedProfileIds): ICommand;

