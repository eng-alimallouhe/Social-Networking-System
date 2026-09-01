using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Jobs.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.Jobs.Commands.UpdateJob;

public sealed record UpdateJobCommand(
    Guid JobId,
    string Title,
    string Description,
    string Location,
    JobType Type,
    decimal MinSalary,
    decimal MaxSalary,
    string CurrencyCode,
    SalaryType SalaryType,
    string KeyResponsibilitiesText
) : ICommand;

internal sealed class UpdateJobCommandHandler : ICommandHandler<UpdateJobCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateJobCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
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

        if (job.ClosedAt.HasValue)
        {
            return Result.Failure(JobStatusCodes.JobAlreadyClosed);
        }

        var isAdministrator = await _dbContext.CompanyAdministrators
            .AnyAsync(ca => ca.CompanyId == job.CompanyId && ca.ProfileId == currentProfileId.Value, cancellationToken);

        if (!isAdministrator)
        {
            return Result.Failure(JobStatusCodes.NotCompanyAdmin);
        }

        if (request.MinSalary < 0 || request.MaxSalary < 0 || request.MinSalary > request.MaxSalary)
        {
            return Result.Failure(JobStatusCodes.InvalidSalaryRange);
        }

        job.Update(
            title: request.Title,
            description: request.Description,
            location: request.Location,
            type: request.Type,
            minSalary: request.MinSalary,
            maxSalary: request.MaxSalary,
            currencyCode: request.CurrencyCode,
            salaryType: request.SalaryType,
            keyResponsibilitiesText: request.KeyResponsibilitiesText);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(JobStatusCodes.JobUpdated);
    }
}
