using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Jobs.Jobs.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Jobs.Jobs.Queries.GetSuggestedJobs;

/// <summary>
/// Represents a query to retrieve suggested job postings for the current authenticated user based on skills and specialization.
/// </summary>
/// <param name="Count">Maximum number of suggested jobs to retrieve.</param>
public sealed record GetSuggestedJobsQuery(int Count = 5) : IQuery<List<JobSummaryDto>>;

internal sealed class GetSuggestedJobsQueryHandler : IQueryHandler<GetSuggestedJobsQuery, List<JobSummaryDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileStorageService _fileStorageService;

    public GetSuggestedJobsQueryHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<List<JobSummaryDto>>> Handle(GetSuggestedJobsQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        var limit = request.Count > 0 && request.Count <= 20 ? request.Count : 5;

        string? specialization = null;
        var profileSkillIds = new List<Guid>();

        if (profileId.HasValue)
        {
            specialization = await _dbContext.Profiles
                .AsNoTracking()
                .Where(p => p.Id == profileId.Value)
                .Select(p => p.Specialization)
                .FirstOrDefaultAsync(cancellationToken);

            profileSkillIds = await _dbContext.ProfileSkills
                .AsNoTracking()
                .Where(ps => ps.ProfileId == profileId.Value)
                .Select(ps => ps.SkillId)
                .ToListAsync(cancellationToken);
        }

        var baseQuery = _dbContext.Jobs
            .AsNoTracking()
            .Where(j => j.IsActive && j.ClosedAt == null);

        var rawItems = await baseQuery
            .OrderByDescending(j => j.CreatedAt)
            .Take(limit * 2)
            .Select(j => new
            {
                j.Id,
                j.CompanyId,
                CompanyName = j.Company.Name,
                CompanyLogoObjectKey = j.Company.LogoObjectKey,
                j.Title,
                j.Description,
                j.Location,
                j.Type,
                j.MinSalary,
                j.MaxSalary,
                j.CurrencyCode,
                j.SalaryType,
                ApplicationsCount = j.Applications.Count(a => a.IsActive),
                SkillIds = _dbContext.JobSkills.Where(js => js.JobId == j.Id).Select(js => js.SkillId).ToList(),
                Skills = _dbContext.JobSkills
                    .Where(js => js.JobId == j.Id)
                    .Select(js => _dbContext.Skills.Where(s => s.Id == js.SkillId).Select(s => s.Name).FirstOrDefault() ?? string.Empty)
                    .Where(name => name != string.Empty)
                    .ToList(),
                j.CreatedAt,
                j.ClosedAt,
                j.IsActive
            })
            .ToListAsync(cancellationToken);

        // Score jobs based on user skills / specialization matches
        var rankedItems = rawItems
            .OrderByDescending(j =>
            {
                int score = 0;
                if (profileSkillIds.Count > 0 && j.SkillIds.Any(sid => profileSkillIds.Contains(sid)))
                {
                    score += 2;
                }
                if (!string.IsNullOrWhiteSpace(specialization) &&
                    j.Title.Contains(specialization, StringComparison.OrdinalIgnoreCase))
                {
                    score += 1;
                }
                return score;
            })
            .ThenByDescending(j => j.CreatedAt)
            .Take(limit)
            .ToList();

        var distinctKeys = rankedItems
            .Select(j => j.CompanyLogoObjectKey)
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Distinct()
            .ToList();

        var urlTasks = distinctKeys.Select(async k => new
        {
            Key = k!,
            Url = await _fileStorageService.GetTemporaryUrlAsync(k!, TimeSpan.FromHours(1))
        });
        var resolvedUrls = await Task.WhenAll(urlTasks);
        var urlMap = resolvedUrls.ToDictionary(r => r.Key, r => r.Url);

        var items = rankedItems.Select(j => new JobSummaryDto(
            Id: j.Id,
            CompanyId: j.CompanyId,
            CompanyName: j.CompanyName,
            CompanyLogoUrl: !string.IsNullOrWhiteSpace(j.CompanyLogoObjectKey) && urlMap.TryGetValue(j.CompanyLogoObjectKey, out var url)
                ? url
                : null,
            Title: j.Title,
            Description: j.Description,
            Location: j.Location,
            Type: j.Type,
            MinSalary: j.MinSalary,
            MaxSalary: j.MaxSalary,
            CurrencyCode: j.CurrencyCode,
            SalaryType: j.SalaryType,
            ApplicationsCount: j.ApplicationsCount,
            Skills: j.Skills,
            CreatedAt: j.CreatedAt,
            ClosedAt: j.ClosedAt,
            IsActive: j.IsActive
        )).ToList();

        return Result<List<JobSummaryDto>>.Success(items, OperationStatusCode.Success);
    }
}
