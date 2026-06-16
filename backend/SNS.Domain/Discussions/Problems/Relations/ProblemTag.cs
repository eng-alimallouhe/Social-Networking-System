using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Discussions.Problems.Relations;

public class ProblemTag : IHardDeletable
{
    //Primary Key:
    public Guid Id { get; private set; }

    // Foreign Key: One(Problem) ? Many(ProblemTags)
    public Guid ProblemId { get; private set; }

    // Foreign Key: One(Tag) ? Many(ProblemTags)
    public Guid TagId { get; private set; }


    public ProblemTag()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
    }
   
    public static ProblemTag Create(Guid problemId, Guid tagId)
    {
        return new ProblemTag
        {
            ProblemId = problemId,
            TagId = tagId
        };
    }
}
