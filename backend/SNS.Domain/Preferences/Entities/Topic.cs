using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Preferences.Entities;

public class Topic : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    public string Name { get; private set; } = default!;

    // Navigation
    public ICollection<TopicInterest> TopicInterests { get; private set; } = new List<TopicInterest>();

    private Topic()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static Topic Create(string name)
    {
        return new Topic
        {
            Name = name
        };
    }
}
