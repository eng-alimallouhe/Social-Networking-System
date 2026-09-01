using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Experiences.Commands.AddResumeExperience;

/// <summary>
/// Represents a command to add a work experience entry to a resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the target resume.</param>
/// <param name="CompanyName">The name of the company or organization.</param>
/// <param name="Position">The job title or position held.</param>
/// <param name="Description">The description of responsibilities and achievements.</param>
/// <param name="StartDate">The start date of employment.</param>
/// <param name="EndDate">The optional end date of employment.</param>
public sealed record AddResumeExperienceCommand(
    Guid ResumeId,
    string CompanyName,
    string Position,
    string Description,
    DateTime StartDate,
    DateTime? EndDate
) : ICommand<Guid>;

/// <summary>
/// Handles the execution of <see cref="AddResumeExperienceCommand"/> to attach a work experience record.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies parent resume existence and ownership.
/// 3. Validates required company and position parameters.
/// 4. Instantiates <see cref="ResumeExperience"/> and persists it via repository.
/// 5. Commits changes via unit of work.
/// Side effects include database insert and transaction commit.
/// </remarks>
internal sealed class AddResumeExperienceCommandHandler : ICommandHandler<AddResumeExperienceCommand, Guid>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeExperience> _experienceRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddResumeExperienceCommandHandler(
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

    public async Task<Result<Guid>> Handle(AddResumeExperienceCommand request, CancellationToken cancellationToken)
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

        if (string.IsNullOrWhiteSpace(request.CompanyName) || string.IsNullOrWhiteSpace(request.Position))
        {
            return Result<Guid>.Failure(OperationStatusCode.InvalidInput);
        }

        var experience = ResumeExperience.Create(
            resumeId: request.ResumeId,
            companyName: request.CompanyName,
            position: request.Position,
            description: request.Description ?? string.Empty,
            startDate: request.StartDate,
            endDate: request.EndDate
        );

        _experienceRepo.Add(experience);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(experience.Id, ResumeStatusCodes.ExperienceAdded);
    }
}
