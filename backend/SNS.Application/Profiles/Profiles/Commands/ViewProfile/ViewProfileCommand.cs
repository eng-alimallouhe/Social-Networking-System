using SNS.Application.Abstractions.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNS.Application.Profiles.Profiles.Commands.ViewProfile;

public sealed record ViewProfileCommand(
    Guid ViewedProfileId
) : ICommand;
