using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Preferences.Entities;

public class Topic : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    // Navigation
    public ICollection<TopicInterest> TopicInterests { get; set; } = new List<TopicInterest>();

    public Topic()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}