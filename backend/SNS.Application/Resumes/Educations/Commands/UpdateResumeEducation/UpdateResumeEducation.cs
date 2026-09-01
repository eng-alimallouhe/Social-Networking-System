using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Educations.Commands.UpdateResumeEducation;

/// <summary>
/// Represents a command to update an existing education entry on a resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the parent resume.</param>
/// <param name="EducationId">The unique identifier of the education entry to update.</param>
/// <param name="UniversityName">The updated university name.</param>
/// <param name="FacultyName">The updated faculty name.</param>
/// <param name="Degree">The updated degree.</param>
/// <param name="FieldOfStudy">The updated field of study.</param>
/// <param name="StartDate">The updated start date.</param>
/// <param name="EndDate">The updated optional end date.</param>
/// <param name="GPA">The updated optional GPA.</param>
public sealed record UpdateResumeEducationCommand(
    Guid ResumeId,
    Guid EducationId,
    string UniversityName,
    string FacultyName,
    string Degree,
    string FieldOfStudy,
    DateTime StartDate,
    DateTime? EndDate,
    double? GPA
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="UpdateResumeEducationCommand"/> to update an education record.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies parent resume existence and ownership.
/// 3. Validates education entry existence and association with the resume.
/// 4. Updates entity properties via domain method.
/// 5. Commits changes via unit of work.
/// Side effects include entity property updates and database commit.
/// </remarks>
internal sealed class UpdateResumeEducationCommandHandler : ICommandHandler<UpdateResumeEducationCommand>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeEducation> _educationRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateResumeEducationCommandHandler(
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

    public async Task<Result> Handle(UpdateResumeEducationCommand request, CancellationToken cancellationToken)
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

        if (string.IsNullOrWhiteSpace(request.UniversityName) || string.IsNullOrWhiteSpace(request.Degree))
        {
            return Result.Failure(OperationStatusCode.InvalidInput);
        }

        var education = await _educationRepo.GetByIdAsync(request.EducationId, cancellationToken);
        if (education == null || education.ResumeId != request.ResumeId)
        {
            return Result.Failure(ResumeStatusCodes.EducationNotFound);
        }

        education.Update(
            request.UniversityName,
            request.FacultyName ?? string.Empty,
            request.Degree,
            request.FieldOfStudy ?? string.Empty,
            request.StartDate,
            request.EndDate,
            request.GPA
        );

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ResumeStatusCodes.EducationUpdated);
    }
}
