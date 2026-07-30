using SNS.Application.Abstractions.Messaging;

namespace SNS.Application.Profiles.Profiles.Commands.UpdateBasicInformation;

/// <summary>
/// Represents a command to update the basic profile information of the authenticated user.
/// </summary>
/// <param name="FullName">The updated full display name.</param>
/// <param name="Bio">The updated biography text.</param>
/// <param name="Specialization">The updated specialization or professional title.</param>
/// <param name="Location">The updated geographic location string.</param>
public sealed record UpdateBasicInformationCommand(
    string FullName,
    string Bio, 
    string Specialization,
    string Location
) : ICommand;

