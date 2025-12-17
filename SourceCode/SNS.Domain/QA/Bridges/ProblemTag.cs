
using SNS.Domain.Preferences.Entities;
using SNS.Domain.QA.Entities;

namespace SNS.Domain.QA.Bridges;

public class ProblemTag
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(Problem) → Many(ProblemTags)
    public Guid ProblemId { get; set; }

    // Foreign Key: One(Tag) → Many(ProblemTags)
    public Guid TagId { get; set; }

    // Navigation Properties
    public Problem Problem { get; set; } = null!;
    public Tag Tag { get; set; } = null!;
}
