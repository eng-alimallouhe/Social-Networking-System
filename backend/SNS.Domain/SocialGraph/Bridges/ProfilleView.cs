using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.SocialGraph.Bridges;

public class ProfileView : ISoftDeletable
{
    public Guid Id { get; set; }
    public Guid ViewedId { get; set; }
    public Guid ViewerId { get; set; }
    public DateTime ViewedAt { get; set; }

    //Soft Delete:
    public bool IsActive { get; set; }




    public ProfileView()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        ViewedAt = DateTime.UtcNow;
        IsActive = true;
    }
}