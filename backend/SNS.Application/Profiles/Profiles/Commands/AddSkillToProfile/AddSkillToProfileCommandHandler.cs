using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Profiles.Profiles.Relations;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Profiles.Profiles.Commands.AddSkillToProfile;

/// <summary>
/// Handles the execution of <see cref="AddSkillToProfileCommand"/> to associate a skill with a user profile.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Validates skill existence and active status in the database.
/// 3. Checks whether the profile already contains the skill.
/// 4. Adds a new <see cref="ProfileSkill"/> entity and persists changes.
/// Side effects include creating and saving a new profile-skill entity.
/// </remarks>
internal sealed class AddSkillToProfileCommandHandler :
    ICommandHandler<AddSkillToProfileCommand>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<ProfileSkill> _profileSkillRepo;
    private readonly IApplicationDbContext _dbContext;
    private readonly IUnitOfWork _unitOfWork;


    public AddSkillToProfileCommandHandler(
        ICurrentUserService currentUserService,
        IRepository<ProfileSkill> profileSkillRepo,
        IApplicationDbContext dbContext,
        IUnitOfWork unitOfWork)
    {
        _currentUserService = currentUserService;
        _profileSkillRepo = profileSkillRepo;
        _dbContext = dbContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddSkillToProfileCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;

        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (!await _dbContext.Skills.AnyAsync(
        s => s.Id == request.SkillId && s.IsActive,
        cancellationToken))
        {
            return Result.Failure(ResourceStatusCode.NotFound);
        }

        var alreadyExists = await _dbContext.ProfileSkills.AnyAsync(
            ps => ps.ProfileId == profileId.Value &&
                  ps.SkillId == request.SkillId,
            cancellationToken);

        if (alreadyExists)
        {
            return Result.Success(OperationStatusCode.Success);
        }

        _profileSkillRepo.Add(
            ProfileSkill.Create(
                profileId: profileId.Value,
                skillId: request.SkillId,
                level: request.ProficiencyLevel));

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}