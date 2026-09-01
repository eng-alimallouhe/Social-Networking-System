using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Educations.Commands.AddResumeEducation;

/// <summary>
/// Represents a command to add an education entry to an existing resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the target resume.</param>
/// <param name="UniversityName">The institution or university name.</param>
/// <param name="FacultyName">The faculty or department name.</param>
/// <param name="Degree">The degree obtained or pursued.</param>
/// <param name="FieldOfStudy">The major field of study.</param>
/// <param name="StartDate">The starting date of study.</param>
/// <param name="EndDate">The optional completion date.</param>
/// <param name="GPA">The optional grade point average.</param>
public sealed record AddResumeEducationCommand(
    Guid ResumeId,
    string UniversityName,
    string FacultyName,
    string Degree,
    string FieldOfStudy,
    DateTime StartDate,
    DateTime? EndDate,
    double? GPA
) : ICommand<Guid>;

/// <summary>
/// Handles the execution of <see cref="AddResumeEducationCommand"/> to create and attach an education record.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies that the parent resume exists and belongs to the authenticated user.
/// 3. Validates required education parameters.
/// 4. Instantiates <see cref="ResumeEducation"/> and adds it to the repository.
/// 5. Commits changes via unit of work.
/// Side effects include database insert and transaction commit.
/// </remarks>
internal sealed class AddResumeEducationCommandHandler : ICommandHandler<AddResumeEducationCommand, Guid>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeEducation> _educationRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddResumeEducationCommandHandler(
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

    public async Task<Result<Guid>> Handle(AddResumeEducationCommand request, CancellationToken cancellationToken)
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

        if (string.IsNullOrWhiteSpace(request.UniversityName) || string.IsNullOrWhiteSpace(request.Degree))
        {
            return Result<Guid>.Failure(OperationStatusCode.InvalidInput);
        }

        var education = ResumeEducation.Create(
            resumeId: request.ResumeId,
            universityName: request.UniversityName,
            facultyName: request.FacultyName ?? string.Empty,
            degree: request.Degree,
            fieldOfStudy: request.FieldOfStudy ?? string.Empty,
            startDate: request.StartDate,
            endDate: request.EndDate,
            gpa: request.GPA
        );

        _educationRepo.Add(education);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(education.Id, ResumeStatusCodes.EducationAdded);
    }
}
