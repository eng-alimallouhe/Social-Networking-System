using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Experiences.Commands.UpdateResumeExperience;

/// <summary>
/// Represents a command to update an existing work experience entry on a resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the parent resume.</param>
/// <param name="ExperienceId">The unique identifier of the experience record to update.</param>
/// <param name="CompanyName">The updated company name.</param>
/// <param name="Position">The updated position title.</param>
/// <param name="Description">The updated description of responsibilities.</param>
/// <param name="StartDate">The updated employment start date.</param>
/// <param name="EndDate">The updated optional employment end date.</param>
public sealed record UpdateResumeExperienceCommand(
    Guid ResumeId,
    Guid ExperienceId,
    string CompanyName,
    string Position,
    string Description,
    DateTime StartDate,
    DateTime? EndDate
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="UpdateResumeExperienceCommand"/> to update an experience record.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies parent resume existence and ownership.
/// 3. Validates experience entry existence and association with the resume.
/// 4. Updates entity properties via domain method.
/// 5. Commits changes via unit of work.
/// Side effects include entity property updates and database commit.
/// </remarks>
internal sealed class UpdateResumeExperienceCommandHandler : ICommandHandler<UpdateResumeExperienceCommand>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeExperience> _experienceRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateResumeExperienceCommandHandler(
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

    public async Task<Result> Handle(UpdateResumeExperienceCommand request, CancellationToken cancellationToken)
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

        if (string.IsNullOrWhiteSpace(request.CompanyName) || string.IsNullOrWhiteSpace(request.Position))
        {
            return Result.Failure(OperationStatusCode.InvalidInput);
        }

        var experience = await _experienceRepo.GetByIdAsync(request.ExperienceId, cancellationToken);
        if (experience == null || experience.ResumeId != request.ResumeId)
        {
            return Result.Failure(ResumeStatusCodes.ExperienceNotFound);
        }

        experience.Update(
            request.CompanyName,
            request.Position,
            request.Description ?? string.Empty,
            request.StartDate,
            request.EndDate
        );

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ResumeStatusCodes.ExperienceUpdated);
    }
}
