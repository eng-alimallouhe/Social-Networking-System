using SNS.Domain.Abstractions.Common;

namespace SNS.Domain.Preferences.Entities;

public class TopicInterest : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Keys
    public Guid InterestId { get; set; }
    public Guid TopicId { get; set; }

    // Navigation
    public Interest Interest { get; set; } = null!;
    public Topic Topic { get; set; } = null!;
}
