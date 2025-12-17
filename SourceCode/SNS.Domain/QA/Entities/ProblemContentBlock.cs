using SNS.Domain.QA.Enums;

namespace SNS.Domain.QA.Entities;

public class ProblemContentBlock
    {
        // Primary Key
        public Guid Id { get; set; }

        // Foreign Key: One(Problem) → Many(ProblemContentBlocks)
        public Guid ProblemId { get; set; }

        // General Properties
        public ProblemBlockType BlockType { get; set; }
        public string Content { get; set; } = string.Empty;
        public string? ExtraInfo { get; set; }
        public int Order { get; set; }

        // Navigation Properties
        public Problem Problem { get; set; } = null!;
    }
