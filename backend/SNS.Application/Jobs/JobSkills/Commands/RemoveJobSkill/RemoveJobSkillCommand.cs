using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Jobs.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.JobSkills.Commands.RemoveJobSkill;

public sealed record RemoveJobSkillCommand(
    Guid JobId,
    Guid SkillId
) : ICommand;

internal sealed class RemoveJobSkillCommandHandler : ICommandHandler<RemoveJobSkillCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<JobSkill> _jobSkillRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveJobSkillCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IRepository<JobSkill> jobSkillRepository,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _jobSkillRepository = jobSkillRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveJobSkillCommand request, CancellationToken cancellationToken)
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
            return Result.Failure(JobSkillStatusCodes.JobNotFound);
        }

        var isAdministrator = await _dbContext.CompanyAdministrators
            .AnyAsync(ca => ca.CompanyId == job.CompanyId && ca.ProfileId == currentProfileId.Value, cancellationToken);

        if (!isAdministrator)
        {
            return Result.Failure(JobSkillStatusCodes.NotCompanyAdmin);
        }

        var jobSkill = await _dbContext.JobSkills
            .FirstOrDefaultAsync(js => js.JobId == request.JobId && js.SkillId == request.SkillId, cancellationToken);

        if (jobSkill == null)
        {
            return Result.Failure(JobSkillStatusCodes.JobSkillNotFound);
        }

        _jobSkillRepository.Delete(jobSkill);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(JobSkillStatusCodes.JobSkillRemoved);
    }
}
