using System.Linq.Expressions;

namespace SNS.Application.Shared.Abstractions.BackgroundJobs;

public interface IJobSchedulerService
{
    void Enqueue(Expression<Func<Task>> job);

    void Enqueue<T>(Expression<Func<T, Task>> job);
}