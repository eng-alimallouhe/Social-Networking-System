using SNS.Domain.Projects.Entities;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.Abstractions.Common;

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
}
