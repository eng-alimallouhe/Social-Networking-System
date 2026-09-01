using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Projects.Enums;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;
using SNS.Domain.Shared.Entities;

namespace SNS.Domain.Projects.Bridges;

public class ProjectContributor : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Keys
    public Guid ProjectId { get; private set; }
    public Guid ContributorId { get; private set; }

    // General Properties
    public InvitingStatus InvitingStatus { get; private set; }
    public ProjectRole Role { get; private set; }

    public DateTime InvitationSentAt { get; private set; }
    public DateTime? RespondedAt { get; private set; }
    
    public string InvitationMessage { get; private set; } = string.Empty;

    // Navigation
    public Profile Contributor { get; set; } = null!;


    private ProjectContributor()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        InvitingStatus = InvitingStatus.Pending;
        InvitationSentAt = DateTime.UtcNow;
    }

    public static ProjectContributor Create(Guid projectId, Guid contributorId, ProjectRole role, string invitationMessage)
    {
        return new ProjectContributor
        {
            ProjectId = projectId,
            ContributorId = contributorId,
            Role = role,
            InvitationMessage = invitationMessage
        };
    }

    public void AcceptInvitation()
    {
        InvitingStatus = InvitingStatus.Accepted;
        RespondedAt = DateTime.UtcNow;
    }

    public void RejectInvitation()
    {
        InvitingStatus = InvitingStatus.Rejected;
        RespondedAt = DateTime.UtcNow;
    }
}
