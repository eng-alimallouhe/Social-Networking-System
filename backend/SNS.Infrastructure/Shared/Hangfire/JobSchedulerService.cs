using Hangfire;
using SNS.Application.Shared.Abstractions.BackgroundJobs;
using System.Linq.Expressions;

namespace SNS.Infrastructure.Shared.BackgroundJobs;

internal sealed class JobSchedulerService : IJobSchedulerService
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public JobSchedulerService(
        IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }

    public void Enqueue(Expression<Func<Task>> job)
    {
        _backgroundJobClient.Enqueue(job);
    }

    public void Enqueue<T>(Expression<Func<T, Task>> job)
    {
        _backgroundJobClient.Enqueue(job);
    }
}