namespace SNS.Application.Identity.ArchiveManagement.Abstractions;

public interface IArchiveCleanupWorker
{
    Task CleanOldArchivesAsync(CancellationToken cancellationToken);
}