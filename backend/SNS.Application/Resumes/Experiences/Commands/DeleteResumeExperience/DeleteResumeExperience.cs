using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Experiences.Commands.DeleteResumeExperience;

/// <summary>
/// Represents a command to remove a work experience entry from a resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the parent resume.</param>
/// <param name="ExperienceId">The unique identifier of the experience record to delete.</param>
public sealed record DeleteResumeExperienceCommand(Guid ResumeId, Guid ExperienceId) : ICommand;

/// <summary>
/// Handles the execution of <see cref="DeleteResumeExperienceCommand"/> to delete an experience record.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies parent resume existence and ownership.
/// 3. Validates experience entry existence and association.
/// 4. Removes the entity from the repository.
/// 5. Commits changes via unit of work.
/// Side effects include hard deletion and database commit.
/// </remarks>
internal sealed class DeleteResumeExperienceCommandHandler : ICommandHandler<DeleteResumeExperienceCommand>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeExperience> _experienceRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteResumeExperienceCommandHandler(
        ISoftDeletableRepository<Resume> resumeRepo,
        IRepository<ResumeExperience> experienceRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _resumeRepo = resumeRepo;
        _experienceRepo = experienceRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteResumeExperienceCommand request, CancellationToken cancellationToken)
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

        var experience = await _experienceRepo.GetByIdAsync(request.ExperienceId, cancellationToken);
        if (experience == null || experience.ResumeId != request.ResumeId)
        {
            return Result.Failure(ResumeStatusCodes.ExperienceNotFound);
        }

        _experienceRepo.Delete(experience);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ResumeStatusCodes.ExperienceDeleted);
    }
}
