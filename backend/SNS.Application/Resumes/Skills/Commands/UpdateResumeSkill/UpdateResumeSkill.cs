using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Bridges;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Resumes.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Skills.Commands.UpdateResumeSkill;

/// <summary>
/// Represents a command to update an existing skill entry on a resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the parent resume.</param>
/// <param name="SkillId">The unique identifier of the skill record to update.</param>
/// <param name="SkillName">The updated skill name.</param>
/// <param name="Level">The updated proficiency level.</param>
public sealed record UpdateResumeSkillCommand(
    Guid ResumeId,
    Guid SkillId,
    string SkillName,
    ResumeSkillLevel Level
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="UpdateResumeSkillCommand"/> to update a skill record.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies parent resume existence and ownership.
/// 3. Validates skill existence, association, and uniqueness constraint.
/// 4. Updates entity properties via domain method.
/// 5. Commits changes via unit of work.
/// Side effects include entity property updates and database commit.
/// </remarks>
internal sealed class UpdateResumeSkillCommandHandler : ICommandHandler<UpdateResumeSkillCommand>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeSkill> _skillRepo;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateResumeSkillCommandHandler(
        ISoftDeletableRepository<Resume> resumeRepo,
        IRepository<ResumeSkill> skillRepo,
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _resumeRepo = resumeRepo;
        _skillRepo = skillRepo;
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateResumeSkillCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var resume = await _resumeRepo.GetByIdAsync(request.ResumeId, cancellationToken);
        if (resume == null || !resume.IsActive)
        {
            return Result.Failure(ResumeStatusCodes.ResumeNotFound);
        }

        if (resume.OwnerId != profileId.Value)
        {
            return Result.Failure(ResumeStatusCodes.NotResumeOwner);
        }

        if (string.IsNullOrWhiteSpace(request.SkillName))
        {
            return Result.Failure(OperationStatusCode.InvalidInput);
        }

        var skill = await _skillRepo.GetByIdAsync(request.SkillId, cancellationToken);
        if (skill == null || skill.ResumeId != request.ResumeId)
        {
            return Result.Failure(ResumeStatusCodes.SkillNotFound);
        }

        var trimmedName = request.SkillName.Trim();

        var exists = await _dbContext.ResumeSkills
            .AnyAsync(s => s.ResumeId == request.ResumeId && s.Id != request.SkillId && s.SkillName.ToLower() == trimmedName.ToLower(), cancellationToken);

        if (exists)
        {
            return Result.Failure(OperationStatusCode.Conflict);
        }

        skill.Update(trimmedName, request.Level);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ResumeStatusCodes.SkillUpdated);
    }
}
