using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Jobs.Entities;
using SNS.Domain.QA.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.JobApplications.Commands.CreateJobApplication;

public sealed record CreateJobApplicationCommand(
    Guid JobId,
    string CoverLetterText,
    Guid? ResumeId = null,
    string? ResumeFileUrl = null
) : ICommand<Guid>;

internal sealed class CreateJobApplicationCommandHandler : ICommandHandler<CreateJobApplicationCommand, Guid>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISoftDeletableRepository<JobApplication> _applicationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateJobApplicationCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        ISoftDeletableRepository<JobApplication> applicationRepository,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _applicationRepository = applicationRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateJobApplicationCommand request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var job = await _dbContext.Jobs
            .FirstOrDefaultAsync(j => j.Id == request.JobId && j.IsActive, cancellationToken);

        if (job == null || job.ClosedAt.HasValue)
        {
            return Result<Guid>.Failure(JobApplicationStatusCodes.JobClosedOrDeleted);
        }

        var alreadyApplied = await _dbContext.JobApplications
            .AnyAsync(a => a.JobId == request.JobId && a.ApplicantId == currentProfileId.Value && a.IsActive, cancellationToken);

        if (alreadyApplied)
        {
            return Result<Guid>.Failure(JobApplicationStatusCodes.DuplicateApplication);
        }

        if (request.ResumeId.HasValue)
        {
            var resume = await _dbContext.Resumes
                .FirstOrDefaultAsync(r => r.Id == request.ResumeId.Value && r.IsActive, cancellationToken);

            if (resume == null)
            {
                return Result<Guid>.Failure(JobApplicationStatusCodes.ResumeNotFound);
            }

            if (resume.OwnerId != currentProfileId.Value)
            {
                return Result<Guid>.Failure(JobApplicationStatusCodes.NotResumeOwner);
            }
        }

        var application = JobApplication.Create(
            applicantId: currentProfileId.Value,
            jobId: request.JobId,
            resumeId: request.ResumeId,
            coverLetterText: request.CoverLetterText,
            resumeFileUrl: request.ResumeFileUrl,
            status: ApplicationStatus.Pending);

        _applicationRepository.Add(application);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(application.Id, JobApplicationStatusCodes.ApplicationCreated);
    }
}
