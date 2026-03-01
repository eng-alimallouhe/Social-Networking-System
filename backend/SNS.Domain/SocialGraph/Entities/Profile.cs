using SNS.Domain.Common.Helpers;
using SNS.Domain.Abstractions.Common;
using SNS.Domain.ProfileContext.Bridges;
using SNS.Domain.SocialGraph.Bridges;

namespace SNS.Domain.SocialGraph;

public class Profile : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(User) ? One(Profile)
    public Guid UserId { get; set; }

    // Foreign Key: One(Faculty) ? Many(Profile) == Optional 
    public Guid? FacultyId { get; set; }
    
    // Foreign Key: One(University) ? Many(Profile) == Optional 
    public Guid? UniversityId { get; set; }


    public string? FullName { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Specialization { get; set; }

    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? FacebookUrl { get; set; }
    public string? XUrl { get; set; }
    public string? Website { get; set; }

    public string? Location { get; set; }
    public string? SkillsSummary { get; set; }


    // Timestamp
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }


    // Soft Delete
    public bool IsActive { get; set; }


    // Navigation
    public ICollection<Follow> Followers { get; set; } 
        = new List<Follow>();   
    
    public ICollection<Follow> Followings { get; set; } 
        = new List<Follow>();  
    
    public ICollection<Block> BlackList { get; set; } 
        = new List<Block>();
    
    public ICollection<ProfileSkill> ProfileSkills { get; set; } 
        = new List<ProfileSkill>();
    
    public ICollection<ProfileTopic> ProfileTopics { get; set; } 
        = new List<ProfileTopic>();
    
    public ICollection<ProfileInterest> ProfileInterests { get; set; } 
        = new List<ProfileInterest>();
    
    public ICollection<ProfileView> Views { get; set; } 
        = new List<ProfileView>();
    
    public ICollection<ProfileView> Vieweds { get; set; } 
        = new List<ProfileView>();

    public Profile()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }
}


