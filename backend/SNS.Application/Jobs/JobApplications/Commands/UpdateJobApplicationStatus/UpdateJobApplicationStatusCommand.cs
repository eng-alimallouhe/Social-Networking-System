using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.QA.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.JobApplications.Commands.UpdateJobApplicationStatus;

public sealed record UpdateJobApplicationStatusCommand(
    Guid ApplicationId,
    ApplicationStatus NewStatus
) : ICommand;

internal sealed class UpdateJobApplicationStatusCommandHandler : ICommandHandler<UpdateJobApplicationStatusCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateJobApplicationStatusCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateJobApplicationStatusCommand request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var application = await _dbContext.JobApplications
            .FirstOrDefaultAsync(a => a.Id == request.ApplicationId && a.IsActive, cancellationToken);

        if (application == null)
        {
            return Result.Failure(JobApplicationStatusCodes.ApplicationNotFound);
        }

        if (application.Status == ApplicationStatus.Withdrawn)
        {
            return Result.Failure(JobApplicationStatusCodes.InvalidStatusTransition);
        }

        var job = await _dbContext.Jobs
            .FirstOrDefaultAsync(j => j.Id == application.JobId, cancellationToken);

        if (job == null)
        {
            return Result.Failure(JobStatusCodes.JobNotFound);
        }

        var isAdministrator = await _dbContext.CompanyAdministrators
            .AnyAsync(ca => ca.CompanyId == job.CompanyId && ca.ProfileId == currentProfileId.Value, cancellationToken);

        if (!isAdministrator)
        {
            return Result.Failure(JobApplicationStatusCodes.NotCompanyAdmin);
        }

        application.ChangeStatus(request.NewStatus);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(JobApplicationStatusCodes.StatusUpdated);
    }
}
