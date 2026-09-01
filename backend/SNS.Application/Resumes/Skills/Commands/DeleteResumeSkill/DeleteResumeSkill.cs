using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Bridges;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Skills.Commands.DeleteResumeSkill;

/// <summary>
/// Represents a command to remove a skill entry from a resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the parent resume.</param>
/// <param name="SkillId">The unique identifier of the skill record to delete.</param>
public sealed record DeleteResumeSkillCommand(Guid ResumeId, Guid SkillId) : ICommand;

/// <summary>
/// Handles the execution of <see cref="DeleteResumeSkillCommand"/> to delete a skill record.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies parent resume existence and ownership.
/// 3. Validates skill entry existence and association.
/// 4. Removes the entity from the repository.
/// 5. Commits changes via unit of work.
/// Side effects include hard deletion and database commit.
/// </remarks>
internal sealed class DeleteResumeSkillCommandHandler : ICommandHandler<DeleteResumeSkillCommand>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeSkill> _skillRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteResumeSkillCommandHandler(
        ISoftDeletableRepository<Resume> resumeRepo,
        IRepository<ResumeSkill> skillRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _resumeRepo = resumeRepo;
        _skillRepo = skillRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteResumeSkillCommand request, CancellationToken cancellationToken)
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

        var skill = await _skillRepo.GetByIdAsync(request.SkillId, cancellationToken);
        if (skill == null || skill.ResumeId != request.ResumeId)
        {
            return Result.Failure(ResumeStatusCodes.SkillNotFound);
        }

        _skillRepo.Delete(skill);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ResumeStatusCodes.SkillDeleted);
    }
}
