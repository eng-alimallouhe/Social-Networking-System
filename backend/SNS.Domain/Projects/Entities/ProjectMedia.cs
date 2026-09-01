using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Enums;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Projects.Entities;

public class ProjectMedia : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Project) ? Many(Media)
    public Guid ProjectId { get; private set; }

    // General Properties
    public string MediaUrl { get; private set; } = string.Empty;
    public string Caption { get; private set; } = string.Empty;
    public MediaType Type { get; private set; }

    public DateTime CreatedAt { get; private set; }

    private ProjectMedia()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public static ProjectMedia Create(Guid projectId, string mediaUrl, string caption, MediaType type)
    {
        return new ProjectMedia
        {
            ProjectId = projectId,
            MediaUrl = mediaUrl,
            Caption = caption,
            Type = type
        };
    }
}
