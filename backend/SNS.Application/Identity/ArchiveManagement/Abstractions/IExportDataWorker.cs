namespace SNS.Application.Identity.ArchiveManagement.Abstractions;

public interface IExportDataWorker
{
    Task ProcessExportAsync(Guid requestId);
}