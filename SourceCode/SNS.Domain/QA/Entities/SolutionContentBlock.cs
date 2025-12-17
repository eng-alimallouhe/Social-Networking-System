using SNS.Domain.QA.Enums;

namespace SNS.Domain.QA.Entities;

public class SolutionContentBlock
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Solution) → Many(SolutionContentBlocks)
    public Guid SolutionId { get; set; }

    // General Properties
    public SolutionBlockType BlockType { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? ExtraInfo { get; set; }
    public int Order { get; set; }

    // Navigation Properties
    public Solution Solution { get; set; } = null!;
}
