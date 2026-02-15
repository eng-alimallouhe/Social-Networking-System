using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.Projects.Entities;

namespace SNS.Domain.Projects.Bridges;

public class ProjectTag : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid ProjectId { get; set; }
    public Guid TagId { get; set; }

    // Navigation
    public Project Project { get; set; } = null!;
    public Tag Tag { get; set; } = null!;

    public ProjectTag()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}