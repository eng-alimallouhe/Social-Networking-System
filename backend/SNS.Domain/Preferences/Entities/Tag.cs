using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Preferences.Entities;

public class Tag : IHardDeletable
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;

    private Tag()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static Tag Create(string name)
    {
        return new Tag
        {
            Name = name
        };
    }
}
