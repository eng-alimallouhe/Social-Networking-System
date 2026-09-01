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

namespace SNS.Application.Resumes.Skills.Commands.AddResumeSkill;

/// <summary>
/// Represents a command to add a skill entry to a resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the target resume.</param>
/// <param name="SkillName">The name of the skill.</param>
/// <param name="Level">The proficiency level in the skill.</param>
public sealed record AddResumeSkillCommand(
    Guid ResumeId,
    string SkillName,
    ResumeSkillLevel Level
) : ICommand<Guid>;

/// <summary>
/// Handles the execution of <see cref="AddResumeSkillCommand"/> to attach a skill entry.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies parent resume existence and ownership.
/// 3. Validates skill parameters and checks for duplicates.
/// 4. Instantiates <see cref="ResumeSkill"/> and persists via repository.
/// 5. Commits changes via unit of work.
/// Side effects include database insert and transaction commit.
/// </remarks>
internal sealed class AddResumeSkillCommandHandler : ICommandHandler<AddResumeSkillCommand, Guid>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeSkill> _skillRepo;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddResumeSkillCommandHandler(
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

    public async Task<Result<Guid>> Handle(AddResumeSkillCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var resume = await _resumeRepo.GetByIdAsync(request.ResumeId, cancellationToken);
        if (resume == null || !resume.IsActive)
        {
            return Result<Guid>.Failure(ResumeStatusCodes.ResumeNotFound);
        }

        if (resume.OwnerId != profileId.Value)
        {
            return Result<Guid>.Failure(ResumeStatusCodes.NotResumeOwner);
        }

        if (string.IsNullOrWhiteSpace(request.SkillName))
        {
            return Result<Guid>.Failure(OperationStatusCode.InvalidInput);
        }

        var trimmedName = request.SkillName.Trim();

        var exists = await _dbContext.ResumeSkills
            .AnyAsync(s => s.ResumeId == request.ResumeId && s.SkillName.ToLower() == trimmedName.ToLower(), cancellationToken);

        if (exists)
        {
            return Result<Guid>.Failure(OperationStatusCode.Conflict);
        }

        var skill = ResumeSkill.Create(
            resumeId: request.ResumeId,
            skillName: trimmedName,
            level: request.Level
        );

        _skillRepo.Add(skill);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(skill.Id, ResumeStatusCodes.SkillAdded);
    }
}
