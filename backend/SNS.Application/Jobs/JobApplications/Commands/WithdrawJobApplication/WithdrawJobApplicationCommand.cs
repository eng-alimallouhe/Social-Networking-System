using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.JobApplications.Commands.WithdrawJobApplication;

public sealed record WithdrawJobApplicationCommand(Guid ApplicationId) : ICommand;

internal sealed class WithdrawJobApplicationCommandHandler : ICommandHandler<WithdrawJobApplicationCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public WithdrawJobApplicationCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(WithdrawJobApplicationCommand request, CancellationToken cancellationToken)
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

        if (application.ApplicantId != currentProfileId.Value)
        {
            return Result.Failure(JobApplicationStatusCodes.NotApplicant);
        }

        application.Withdraw();
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(JobApplicationStatusCodes.ApplicationWithdrawn);
    }
}
