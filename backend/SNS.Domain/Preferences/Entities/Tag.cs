using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;

namespace SNS.Domain.Preferences.Entities;

public class Tag : IHardDeletable
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public Tag()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}