using SNS.Domain.Abstractions.Common;
using SNS.Domain.Common.Helpers;
using SNS.Domain.Content.Entities;
using SNS.Domain.Education.Entities;
using SNS.Domain.Jobs.Entities;
using SNS.Domain.ProfileContext.Bridges;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Projects.Entities;
using SNS.Domain.QA.Bridges;
using SNS.Domain.QA.Entities;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.SocialGraph.Bridges;

namespace SNS.Domain.SocialGraph;

public class Profile : ISoftDeletable
{
    // Primary Key
    public Guid Id { get; set; }

    // Foreign Key: One(User) → One(Profile)
    public Guid UserId { get; set; }

    // Foreign Key: One(Faculty) → Many(Profile) == Optional 
    public Guid? FacultyId { get; set; }
    
    // Foreign Key: One(University) → Many(Profile) == Optional 
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


    // Navigation Properties (Optional Relationships)
    public Faculty? Faculty { get; set; }
    public University? University { get; set; }


    // Navigation
    public ICollection<Follow> Followers { get; set; } = new List<Follow>();   
    public ICollection<Follow> Followings { get; set; } = new List<Follow>();  
    public ICollection<Block> BlackList { get; set; } = new List<Block>();
    public ICollection<ProfileSkill> ProfileSkills { get; set; } = new List<ProfileSkill>();
    public ICollection<ProfileSkillRequest> ProfileSkillRequests { get; set; } = new List<ProfileSkillRequest>();
    public ICollection<ProfileTopic> ProfileTopics { get; set; } = new List<ProfileTopic>();
    public ICollection<Resume> Resumes { get; set; } = new List<Resume>();
    public ICollection<UniversityRequest> UniversityRequests { get; set; } = new List<UniversityRequest>();
    public ICollection<FacultyRequest> FacultyRequests { get; set; } = new List<FacultyRequest>();
    public ICollection<ProfileUniversityRequest> ProfileUniversityRequests { get; set; } = new List<ProfileUniversityRequest>();
    public ICollection<ProfileFacultyRequest> ProfileFacultyRequests { get; set; } = new List<ProfileFacultyRequest>();
    public ICollection<JobApplication> JobApplications { get; set; } = new List<JobApplication>();
    public ICollection<Job> Jobs { get; set; } = new List<Job>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();
    public ICollection<PostReaction> PostReactions { get; set; } = new List<PostReaction>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<CommentReaction> CommentReactions { get; set; } = new List<CommentReaction>();
    public ICollection<ProfileView> Views { get; set; } = new List<ProfileView>();
    public ICollection<ProfileView> Vieweds { get; set; } = new List<ProfileView>();
    public ICollection<Project> Projects { get; set; } = new List<Project>();
    public ICollection<ProjectContributor> ProjectContributors { get; set; } = new List<ProjectContributor>();
    public ICollection<ProjectView> ProjectViews { get; set; } = new List<ProjectView>();
    public ICollection<Problem> Problems { get; set; } = new List<Problem>();
    public ICollection<Solution> Solutions { get; set; } = new List<Solution>();
    public ICollection<ProblemVote> ProblemVotes { get; set; } = new List<ProblemVote>();
    public ICollection<SolutionVote> SolutionVotes { get; set; } = new List<SolutionVote>();
    public ICollection<ProblemView> ProblemViews { get; set; } = new List<ProblemView>();

    public Profile()
    {
        Id = SequentialGuid.GenerateSequentialGuid();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        IsActive = true;
    }
}
