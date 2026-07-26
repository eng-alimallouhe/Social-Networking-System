using SNS.Application.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Data;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Profiles.Profiles.Commands.RemoveSkillFromProfile;

public sealed record RemoveSkillFromProfileCommand(
    Guid SkillId    
): ICommand;
