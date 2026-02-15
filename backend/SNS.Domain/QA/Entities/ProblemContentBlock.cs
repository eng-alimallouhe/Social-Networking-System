using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.QA.Enums;

namespace SNS.Domain.QA.Entities;

public class ProblemContentBlock : IHardDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Problem) → Many(ProblemContentBlocks)
    public Guid ProblemId { get; set; }

    // General Properties
    public ProblemBlockType Type { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ExtraInfo { get; set; }
    public int Order { get; set; }

    // Navigation Properties
    public Problem Problem { get; set; } = null!;

    public ProblemContentBlock()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
}