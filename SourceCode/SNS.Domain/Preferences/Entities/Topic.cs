using SNS.Domain.Abstractions.Common;

namespace SNS.Domain.Preferences.Entities;

public class Topic : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    public string TopicTitle { get; set; } = default!;

    // Navigation
    public ICollection<TopicInterest> TopicInterests { get; set; } = new List<TopicInterest>();
}
