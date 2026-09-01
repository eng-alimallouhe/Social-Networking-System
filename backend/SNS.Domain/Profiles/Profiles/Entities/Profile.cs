using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.Discussions.Problems.Entities;
using SNS.Domain.Educations.Entities;
using SNS.Domain.Identity.Users.Constants;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Jobs.Entities;
using SNS.Domain.Profiles.Profiles.Relations;
using SNS.Domain.Profiles.SocialGraph.Entities;
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
    public string? ProfilePictureObjectKey { get; private set; }
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

    public ICollection<Problem> Problems { get; private set; } = new List<Problem>();
    
    public ICollection<Post> Posts { get; private set; } = new List<Post>();

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
    
    public ICollection<ProfileTag> ProfileTags { get; private set; } 
        = new List<ProfileTag>();

    
    public ICollection<ProfileView> Views { get; private set; } 
        = new List<ProfileView>();
    

    public ICollection<ProfileView> Vieweds { get; private set; } 
        = new List<ProfileView>();
    

    public ICollection<CommunityMembership> Memberships { get; private set; } 
        = new List<CommunityMembership>();
    
    public ICollection<AcademicRecord> AcademicRecords { get; private set; } 
        = new List<AcademicRecord>();

    public ICollection<CompanyCreateRequest> CompanyCreateRequests { get; private set; } 
        = new List<CompanyCreateRequest>();

    private Profile()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public static Profile Create(Guid userId, string fullName, string? bio = null, string? profilePictureObjectKey = null, string? specialization = null, string? gitHubUrl = null, string? linkedInUrl = null, string? facebookUrl = null, string? xUrl = null, string? website = null, string? location = null, string? skillsSummary = null)
    {
        var entity = new Profile();
        entity.UserId = userId;
        entity.FullName = fullName;
        entity.Bio = bio;
        entity.ProfilePictureObjectKey = profilePictureObjectKey;
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

    public static Profile CreateDefaultProfile()
    {
        var entity = new Profile();

        entity.UserId = SystemUsers.GhostUserId;
        entity.Id = SystemProfiles.GhostProfileId;
        entity.Specialization = "Default";
        entity.FullName = SystemProfiles.GhostProfileFullName;
        entity.ProfilePictureObjectKey = SystemProfiles.GhostProfilePictureUrl;
        entity.CreatedAt = new DateTime(1, 1, 1);
        entity.UpdatedAt = new DateTime(1, 1, 1);
        entity.Reputation = 9999999;
        
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

    public void UpdateProfilePictureObjectKey(string profilePictureObjectKey)
    {
        this.ProfilePictureObjectKey = profilePictureObjectKey;
        this.UpdatedAt = DateTime.UtcNow;
    }


    public void UpdateBasicInformation(string fullName, string bio, string specialization, string location)
    {
        this.FullName = fullName;
        this.Bio = bio;
        this.Specialization = specialization;
        this.Location = location;
    }

    public void UpdateSocialLinks(
        string? gitHubUrl,
        string? linkedInUrl,
        string? facebookUrl,
        string website,
        string? xUrl)
    {
        this.GitHubUrl = gitHubUrl;
        this.LinkedInUrl = linkedInUrl;
        this.FacebookUrl = facebookUrl;
        this.Website = website;
        this.XUrl = xUrl;
    }

    /// <summary>
    /// Safely adjusts the profile's accumulated reputation points.
    /// </summary>
    /// <param name="pointsDelta">The points to add (positive) or deduct (negative).</param>
    public void AdjustReputation(int pointsDelta)
    {
        Reputation = Math.Max(0, Reputation + pointsDelta);
        UpdatedAt = DateTime.UtcNow;
    }
}