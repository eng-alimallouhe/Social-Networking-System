using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;

namespace SNS.Domain.Identity.ArchiveManagement.Entities;

public enum ExportStatus { Pending, Processing, Completed, Failed }

public class ExportDataRequest : Entity, IHardDeletable
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public ExportStatus Status { get; private set; }
    public string? DownloadUrl { get; private set; } 
    public DateTime CreatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private ExportDataRequest() { }

    public static ExportDataRequest Create(Guid userId)
    {
        return new ExportDataRequest
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Status = ExportStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void MoveToProcessing() => Status = ExportStatus.Processing;

    public void Complete(string downloadUrl)
    {
        Status = ExportStatus.Completed;
        DownloadUrl = downloadUrl;
        CompletedAt = DateTime.UtcNow;
    }

    public void Fail() => Status = ExportStatus.Failed;
}