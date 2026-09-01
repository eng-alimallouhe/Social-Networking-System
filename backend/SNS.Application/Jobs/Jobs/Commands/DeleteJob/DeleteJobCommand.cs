using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Jobs.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.Jobs.Commands.DeleteJob;

public sealed record DeleteJobCommand(Guid JobId) : ICommand;

internal sealed class DeleteJobCommandHandler : ICommandHandler<DeleteJobCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISoftDeletableRepository<Job> _jobRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteJobCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        ISoftDeletableRepository<Job> jobRepository,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _jobRepository = jobRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteJobCommand request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var job = await _dbContext.Jobs
            .FirstOrDefaultAsync(j => j.Id == request.JobId && j.IsActive, cancellationToken);

        if (job == null)
        {
            return Result.Failure(JobStatusCodes.JobNotFound);
        }

        var isAdministrator = await _dbContext.CompanyAdministrators
            .AnyAsync(ca => ca.CompanyId == job.CompanyId && ca.ProfileId == currentProfileId.Value, cancellationToken);

        if (!isAdministrator)
        {
            return Result.Failure(JobStatusCodes.NotCompanyAdmin);
        }

        _jobRepository.SoftDelete(job);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(JobStatusCodes.JobDeleted);
    }
}
