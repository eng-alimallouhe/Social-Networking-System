using SNS.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNS.Application.Profiles.Profiles.Commands.ViewProfile;

/// <summary>
/// Represents a command to record a profile view event by the authenticated user.
/// </summary>
/// <param name="ViewedProfileId">The unique identifier of the target profile being viewed.</param>
public sealed record ViewProfileCommand(
    Guid ViewedProfileId
) : ICommand;

