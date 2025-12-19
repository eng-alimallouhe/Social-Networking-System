using SNS.Domain.Abstractions.Common;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Projects.Enums;
using SNS.Domain.SocialGraph;

namespace SNS.Domain.Projects.Bridges;

public class ProjectContributor : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid ProjectId { get; set; }
    public Guid ContributorProfileId { get; set; }

    // General Properties
    public InvitingStatus InvitingStatus { get; set; }
    public ProjectRole Role { get; set; }
    public DateTime InvitationSentAt { get; set; }
    public DateTime? RespondedAt { get; set; }
    public string InvitationMessage { get; set; } = string.Empty;

    // Navigation
    public Project Project { get; set; } = null!;
    public Profile ContributorProfile { get; set; } = null!;
}
