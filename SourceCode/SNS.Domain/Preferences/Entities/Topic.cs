namespace SNS.Domain.Preferences.Entities;

public class Topic
{
    // Primary Key
    public Guid Id { get; set; }

    public string TopicTitle { get; set; } = default!;

    public bool IsActive { get; set; } = true; // Soft Delete

    // Navigation
    public ICollection<TopicInterest> TopicInterests { get; set; } = new List<TopicInterest>();
}
