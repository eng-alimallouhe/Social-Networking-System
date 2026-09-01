using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Jobs.Entities;
using SNS.Domain.Jobs.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.Jobs.Commands.CreateJob;

public sealed record CreateJobCommand(
    Guid CompanyId,
    string Title,
    string Description,
    string Location,
    JobType Type,
    decimal MinSalary,
    decimal MaxSalary,
    string CurrencyCode,
    SalaryType SalaryType,
    string KeyResponsibilitiesText,
    List<Guid>? SkillIds = null
) : ICommand<Guid>;

internal sealed class CreateJobCommandHandler : ICommandHandler<CreateJobCommand, Guid>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISoftDeletableRepository<Job> _jobRepository;
    private readonly IRepository<JobSkill> _jobSkillRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateJobCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        ISoftDeletableRepository<Job> jobRepository,
        IRepository<JobSkill> jobSkillRepository,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _jobRepository = jobRepository;
        _jobSkillRepository = jobSkillRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateJobCommand request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var company = await _dbContext.Companies
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId && c.IsActive, cancellationToken);

        if (company == null)
        {
            return Result<Guid>.Failure(JobStatusCodes.CompanyNotActive);
        }

        var isAdministrator = await _dbContext.CompanyAdministrators
            .AnyAsync(ca => ca.CompanyId == request.CompanyId && ca.ProfileId == currentProfileId.Value, cancellationToken);

        if (!isAdministrator)
        {
            return Result<Guid>.Failure(JobStatusCodes.NotCompanyAdmin);
        }

        if (request.MinSalary < 0 || request.MaxSalary < 0 || request.MinSalary > request.MaxSalary)
        {
            return Result<Guid>.Failure(JobStatusCodes.InvalidSalaryRange);
        }

        var job = Job.Create(
            title: request.Title,
            description: request.Description,
            companyId: request.CompanyId,
            location: request.Location,
            type: request.Type,
            minSalary: request.MinSalary,
            maxSalary: request.MaxSalary,
            currencyCode: request.CurrencyCode,
            salaryType: request.SalaryType,
            keyResponsibilitiesText: request.KeyResponsibilitiesText);

        _jobRepository.Add(job);

        if (request.SkillIds != null && request.SkillIds.Count > 0)
        {
            var validSkillIds = await _dbContext.Skills
                .Where(s => request.SkillIds.Contains(s.Id))
                .Select(s => s.Id)
                .ToListAsync(cancellationToken);

            foreach (var skillId in validSkillIds.Distinct())
            {
                var jobSkill = JobSkill.Create(job.Id, skillId);
                _jobSkillRepository.Add(jobSkill);
            }
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(job.Id, JobStatusCodes.JobCreated);
    }
}
