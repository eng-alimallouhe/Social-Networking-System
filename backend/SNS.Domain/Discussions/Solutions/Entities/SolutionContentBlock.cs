using SNS.Domain.Discussions.Solutions.Enums;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Discussions.Solutions.Entities;

public class SolutionContentBlock : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Solution) ? Many(SolutionContentBlocks)
    public Guid SolutionId { get; private set; }

    // General Properties
    public SolutionBlockType Type { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public string? ExtraInfo { get; private set; }

    //the order with the SolutionId must be unique
    //I mean in the same solution you can not add a two blocks with the same order
    public int Order { get; private set; }

    private SolutionContentBlock()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static SolutionContentBlock Create(Guid solutionId, SolutionBlockType type, string content, string? extraInfo, int order)
    {
        var entity = new SolutionContentBlock();
        entity.SolutionId = solutionId;
        entity.Type = type;
        entity.Content = content;
        entity.ExtraInfo = extraInfo;
        entity.Order = order;
        return entity;
    }
}
