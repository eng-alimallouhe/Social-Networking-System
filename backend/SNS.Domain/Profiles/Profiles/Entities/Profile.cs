using SNS.Domain.ContentManagement.Comments.Entities;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.Discussions.Problems.Entities;
using SNS.Domain.Discussions.Problems.Relations;
using SNS.Domain.Discussions.Solutions.Entities;
using SNS.Domain.Educations.Entities;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Profiles.Profiles.Relations;
using SNS.Domain.Profiles.SocialGraph.Entities;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Shared.Abstractions.IDeletable;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Helpers;

namespace SNS.Domain.Profiles.Profiles.Entities;

public class Profile : Entity, ISoftDeletable
{
    // Primary Key
    public Guid Id { get; private set; }

    // Foreign Key: One(User) ? One(Profile)
    // This is Uninque
    public Guid UserId { get; private set; }


    public string FullName { get; private set; } = string.Empty;
    public string? Bio { get; private set; }
    public string? ProfilePictureUrl { get; private set; }
    public string? Specialization { get; private set; }

    public string? GitHubUrl { get; private set; }
    public string? LinkedInUrl { get; private set; }
    public string? FacebookUrl { get; private set; }
    public string? XUrl { get; private set; }
    public string? Website { get; private set; }

    public string? Location { get; private set; }
    public string? SkillsSummary { get; private set; }

    //Reputation System
    public int Reputation { get; private set; }

    public ICollection<ReputationLedger> ReputationHistory { get; private set; } = new List<ReputationLedger>();


    // Timestamp
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }


    // Soft Delete  
    public bool IsActive { get; private set; }


    // Navigation
    public User Owner { get; set; } = null!;
    public ICollection<Follow> Followers { get; private set; } 
        = new List<Follow>();   
    
    public ICollection<Follow> Followings { get; private set; } 
        = new List<Follow>();  
    
    public ICollection<Block> BlackList { get; private set; } 
        = new List<Block>();
    
    public ICollection<ProfileSkill> ProfileSkills { get; private set; } 
        = new List<ProfileSkill>();
    
    public ICollection<ProfileTopic> ProfileTopics { get; private set; } 
        = new List<ProfileTopic>();

    public ICollection<Solution> Solutions { get; private set; }
        = new List<Solution>();

    public ICollection<Post> Posts { get; private set; }
        = new List<Post>();

    public ICollection<Comment> Comments { get; private set; }
        = new List<Comment>();

    public ICollection<Problem> Problems { get; private set; }
        = new List<Problem>();

    public ICollection<ProblemVote> ProblemVotes { get; private set; }
        = new List<ProblemVote>();


    public ICollection<ProfileView> Views { get; private set; } 
        = new List<ProfileView>();
    

    public ICollection<ProfileView> Vieweds { get; private set; } 
        = new List<ProfileView>();


    public ICollection<SavedProfile> SavedProfiles { get; private set; } 
        = new List<SavedProfile>();

    public ICollection<SavedProfile> SavedByProfiles { get; private set; } 
        = new List<SavedProfile>();


    public ICollection<AcademicRecord> AcademicRecords { get; private set; } 
        = new List<AcademicRecord>();

    public ICollection<Project> Projects { get; private set; } = 
        new List<Project>();

    public ICollection<ProjectContributor> ProjectContributors = 
        new List<ProjectContributor>();

    private Profile()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static Profile Create(Guid userId, string fullName, string? bio, string? profilePictureUrl, string? specialization, string? gitHubUrl, string? linkedInUrl, string? facebookUrl, string? xUrl, string? website, string? location, string? skillsSummary)
    {
        var entity = new Profile();
        entity.UserId = userId;
        entity.FullName = fullName;
        entity.Bio = bio;
        entity.ProfilePictureUrl = profilePictureUrl;
        entity.Specialization = specialization;
        entity.GitHubUrl = gitHubUrl;
        entity.LinkedInUrl = linkedInUrl;
        entity.FacebookUrl = facebookUrl;
        entity.XUrl = xUrl;
        entity.Website = website;
        entity.Location = location;
        entity.SkillsSummary = skillsSummary;
        return entity;
    }

    public static Profile CreateSystemProfile(Guid id, Guid userId, string fullName, string? profilePictureUrl)
    {
        var entity = new Profile();
        entity.Id = id;
        entity.UserId = userId;
        entity.FullName = fullName;
        entity.ProfilePictureUrl = profilePictureUrl;
        entity.CreatedAt = new DateTime(year: 1999, month: 12, day: 1);
        entity.UpdatedAt = new DateTime(year: 1999, month: 12, day: 1);
        return entity;
    }

    public void SoftDelete()
    {
        this.IsActive = false;
    }

    public void Activate()
    {
        this.IsActive = true;
    }
}


