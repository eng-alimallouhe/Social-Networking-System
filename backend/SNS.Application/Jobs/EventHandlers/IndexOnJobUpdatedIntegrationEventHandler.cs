using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Search.Jobs.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.Jobs.Events;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Jobs.EventHandlers;

/// <summary>
/// Handles <see cref="JobUpdatedIntegrationEvent"/> to synchronize updated job posting details with the search index.
/// </summary>
public class IndexOnJobUpdatedIntegrationEventHandler
    : INotificationHandler<DomainEventNotification<JobUpdatedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IJobSearchService _jobSearchService;
    private readonly IAppLogger<IndexOnJobUpdatedIntegrationEventHandler> _logger;

    public IndexOnJobUpdatedIntegrationEventHandler(
        IApplicationDbContext dbContext,
        IJobSearchService jobSearchService,
        IAppLogger<IndexOnJobUpdatedIntegrationEventHandler> logger)
    {
        _dbContext = dbContext;
        _jobSearchService = jobSearchService;
        _logger = logger;
    }

    public async Task Handle(DomainEventNotification<JobUpdatedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var jobId = notification.DomainEvent.JobId;

        var jobDocument = await _dbContext.Jobs
            .AsNoTracking()
            .Where(j => j.Id == jobId)
            .Select(j => new JobsDocument
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                Location = j.Location,
                Type = j.Type,
                MinSalary = j.MinSalary,
                MaxSalary = j.MaxSalary,
                CurrencyCode = j.CurrencyCode,
                SalaryType = j.SalaryType,
                CompanyName = j.Company.Name,
                CreatedAt = j.CreatedAt,
                ClosedAt = j.ClosedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (jobDocument == null)
        {
            _logger.LogWarning("Job not found for search index update: {JobId}", jobId);
            return;
        }

        var result = await _jobSearchService.UpsertJobAsync(jobDocument, cancellationToken);
        if (!result.IsSuccess)
        {
            _logger.LogWarning("Failed to update job in search index: {JobId}", jobId);
        }
    }
}
