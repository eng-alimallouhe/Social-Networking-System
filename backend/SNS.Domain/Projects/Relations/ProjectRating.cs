using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Projects.Bridges;

public class ProjectRating : IHardDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Keys
    public Guid RaterId { get; private set; }
    public Guid ProjectId { get; private set; }

    // General
    public int RatingValue { get; private set; }
    public string Comment { get; private set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; private set; }

    // Navigation



    private ProjectRating()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public static ProjectRating Create(Guid raterId, Guid projectId, int ratingValue, string comment)
    {
        return new ProjectRating
        {
            RaterId = raterId,
            ProjectId = projectId,
            RatingValue = ratingValue,
            Comment = comment
        };
    }
}
