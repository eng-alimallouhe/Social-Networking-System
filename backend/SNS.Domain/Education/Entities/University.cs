using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Helpers;
using SNS.Shared.Exceptions;

namespace SNS.Domain.Educations.Entities;

public class University : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Properties
    public string Name { get; private set; } = string.Empty;
    public string Country { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;

    // Timestamp
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // Soft Delete
    public bool IsActive { get; private set; }


    private University()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static University Create(string name, string country, string city)
    {
        return new University
        {
            Name = name,
            Country = country,
            City = city
        };
    }

    public void SoftDelete()
    {
        if (IsActive)
        {
            throw new DomainException("University cannot be deleted because it is active.");
        }
        IsActive = false;
    }
}
