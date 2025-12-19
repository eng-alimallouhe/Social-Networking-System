using SNS.Domain.Abstractions.Common;

namespace SNS.Domain.Preferences.Entities;

public class Tag : IHardDeletable
{
    public int Id { get; set; }
    public required string Name { get; set; }
}