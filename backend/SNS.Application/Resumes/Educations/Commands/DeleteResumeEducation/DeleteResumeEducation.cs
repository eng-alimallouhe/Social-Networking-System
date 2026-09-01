using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Educations.Commands.DeleteResumeEducation;

/// <summary>
/// Represents a command to remove an education entry from a resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the parent resume.</param>
/// <param name="EducationId">The unique identifier of the education record to delete.</param>
public sealed record DeleteResumeEducationCommand(Guid ResumeId, Guid EducationId) : ICommand;

/// <summary>
/// Handles the execution of <see cref="DeleteResumeEducationCommand"/> to delete an education record.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies parent resume existence and ownership.
/// 3. Validates education entry existence and association.
/// 4. Removes the entity from the repository.
/// 5. Commits changes via unit of work.
/// Side effects include hard deletion and database commit.
/// </remarks>
internal sealed class DeleteResumeEducationCommandHandler : ICommandHandler<DeleteResumeEducationCommand>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeEducation> _educationRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteResumeEducationCommandHandler(
        ISoftDeletableRepository<Resume> resumeRepo,
        IRepository<ResumeEducation> educationRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _resumeRepo = resumeRepo;
        _educationRepo = educationRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteResumeEducationCommand request, CancellationToken cancellationToken)
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

        var education = await _educationRepo.GetByIdAsync(request.EducationId, cancellationToken);
        if (education == null || education.ResumeId != request.ResumeId)
        {
            return Result.Failure(ResumeStatusCodes.EducationNotFound);
        }

        _educationRepo.Delete(education);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ResumeStatusCodes.EducationDeleted);
    }
}
