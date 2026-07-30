using SNS.Application.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Profiles.Profiles.Commands.RemoveSkillFromProfile;

/// <summary>
/// Represents a command to remove a skill from the authenticated user's profile.
/// </summary>
/// <param name="SkillId">The unique identifier of the skill association to be removed.</param>
public sealed record RemoveSkillFromProfileCommand(
    Guid SkillId    
): ICommand;

