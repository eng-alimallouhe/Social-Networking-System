using Quartz;
using SNS.Application.Identity.ArchiveManagement.Abstractions;

namespace SNS.Infrastructure.Shared.BackgroundJobs;

[DisallowConcurrentExecution]
public class GdprExportJob : IJob
{
    private readonly IExportDataWorker _exportWorker;

    public GdprExportJob(IExportDataWorker exportWorker)
    {
        _exportWorker = exportWorker;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        var dataMap = context.MergedJobDataMap;
        
        if (dataMap.TryGetValue("RequestId", out var requestIdObj) && requestIdObj is Guid requestId)
        {
            await _exportWorker.ProcessExportAsync(requestId, context.CancellationToken);
        }
    }
}