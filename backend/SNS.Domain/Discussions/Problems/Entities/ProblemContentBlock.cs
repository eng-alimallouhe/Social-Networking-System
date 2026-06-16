using SNS.Domain.Discussions.Problems.Enums;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Discussions.Problems.Entities;

public class ProblemContentBlock : Entity, IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(Problem) ? Many(ProblemContentBlocks)
    public Guid ProblemId { get; private set; }

    // General Properties
    public ProblemBlockType Type { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public string? ExtraInfo { get; private set; }

    //the order with the ProblemId must be unique
    //I mean in the same problem you can not add a two blocks with the same order
    public int Order { get; private set; }

    private ProblemContentBlock()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }

    public static ProblemContentBlock Create(Guid problemId, ProblemBlockType type, string content, string? extraInfo, int order)
    {
        var entity = new ProblemContentBlock();
        entity.ProblemId = problemId;
        entity.Type = type;
        entity.Content = content;
        entity.ExtraInfo = extraInfo;
        entity.Order = order;
        return entity;
    }
}
