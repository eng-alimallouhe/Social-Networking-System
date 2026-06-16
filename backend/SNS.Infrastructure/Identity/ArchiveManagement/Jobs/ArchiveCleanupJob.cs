using Quartz;
using SNS.Application.Identity.ArchiveManagement.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNS.Infrastructure.Identity.ArchiveManagement.Jobs;

[DisallowConcurrentExecution]
public class ArchiveCleanupJob : IJob
{
    private readonly IArchiveCleanupWorker _cleanupWorker;

    public ArchiveCleanupJob(IArchiveCleanupWorker cleanupWorker)
    {
        _cleanupWorker = cleanupWorker;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _cleanupWorker.CleanOldArchivesAsync(context.CancellationToken);
    }
}