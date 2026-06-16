namespace SNS.Application.Shared.Abstractions.BackgroundJobs;

public interface IJobSchedulerService
{
    Task TriggerExportJobAsync(Guid requestId);
}