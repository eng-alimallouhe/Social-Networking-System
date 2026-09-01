using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Jobs.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.JobSkills.Commands.AddJobSkill;

public sealed record AddJobSkillCommand(
    Guid JobId,
    Guid SkillId
) : ICommand<Guid>;

internal sealed class AddJobSkillCommandHandler : ICommandHandler<AddJobSkillCommand, Guid>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<JobSkill> _jobSkillRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddJobSkillCommandHandler(
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

    public async Task<Result<Guid>> Handle(AddJobSkillCommand request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var job = await _dbContext.Jobs
            .FirstOrDefaultAsync(j => j.Id == request.JobId && j.IsActive, cancellationToken);

        if (job == null)
        {
            return Result<Guid>.Failure(JobSkillStatusCodes.JobNotFound);
        }

        var isAdministrator = await _dbContext.CompanyAdministrators
            .AnyAsync(ca => ca.CompanyId == job.CompanyId && ca.ProfileId == currentProfileId.Value, cancellationToken);

        if (!isAdministrator)
        {
            return Result<Guid>.Failure(JobSkillStatusCodes.NotCompanyAdmin);
        }

        var skillExists = await _dbContext.Skills
            .AnyAsync(s => s.Id == request.SkillId, cancellationToken);

        if (!skillExists)
        {
            return Result<Guid>.Failure(JobSkillStatusCodes.SkillNotFound);
        }

        var exists = await _dbContext.JobSkills
            .AnyAsync(js => js.JobId == request.JobId && js.SkillId == request.SkillId, cancellationToken);

        if (exists)
        {
            return Result<Guid>.Failure(JobSkillStatusCodes.JobSkillAlreadyExists);
        }

        var jobSkill = JobSkill.Create(request.JobId, request.SkillId);
        _jobSkillRepository.Add(jobSkill);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(jobSkill.Id, JobSkillStatusCodes.JobSkillAdded);
    }
}
