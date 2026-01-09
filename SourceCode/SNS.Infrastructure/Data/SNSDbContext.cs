using Microsoft.EntityFrameworkCore;
using SNS.Domain.Communities.Entities;
using SNS.Domain.Content.Entities;
using SNS.Domain.Education.Entities;
using SNS.Domain.Jobs.Entities;
using SNS.Domain.Posts.Bridges;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.ProfileContext.Bridges;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Projects.Entities;
using SNS.Domain.QA.Bridges;
using SNS.Domain.QA.Entities;
using SNS.Domain.Resumes.Bridges;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Security;
using SNS.Domain.Security.Entities;
using SNS.Domain.SocialGraph;
using SNS.Domain.SocialGraph.Bridges;
using System.Reflection;

namespace SNS.Infrastructure.Data;

public class SNSDbContext : DbContext
{
    public SNSDbContext(DbContextOptions<SNSDbContext> options) : base(options)
    {
    }


    // 📦 Communities
    public DbSet<Community> Communities { get; set; }
    public DbSet<CommunityAuditLog> CommunityAuditLogs { get; set; }
    public DbSet<CommunityCreationRequest> CommunityCreationRequests { get; set; }
    public DbSet<CommunityInvitation> CommunityInvitations { get; set; }
    public DbSet<CommunityJoinRequest> CommunityJoinRequests { get; set; }
    public DbSet<CommunityMembership> CommunityMemberships { get; set; }
    public DbSet<CommunityRule> CommunityRules { get; set; }
    public DbSet<CommunitySettings> CommunitySettings { get; set; }

    // 📦 Education
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<FacultyRequest> FacultyRequests { get; set; }
    public DbSet<ProfileFacultyRequest> ProfileFacultyRequests { get; set; }
    public DbSet<ProfileUniversityRequest> ProfileUniversityRequests { get; set; }
    public DbSet<University> Universities { get; set; }
    public DbSet<UniversityRequest> UniversityRequests { get; set; }

    // 📦 Jobs
    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<JobSkill> JobSkills { get; set; }

    // 📦 Posts
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentReaction> CommentReactions { get; set; }
    public DbSet<PostMedia> PostMedia { get; set; }
    public DbSet<PostReaction> PostReactions { get; set; }
    // Bridges
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<PostTopic> PostTopics { get; set; }
    public DbSet<PostView> PostViews { get; set; }

    // 📦 Preferences
    public DbSet<Interest> Interests { get; set; }
    public DbSet<InterestCategory> InterestCategories { get; set; }
    public DbSet<InterestRequest> InterestRequests { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<SkillRequest> SkillRequests { get; set; }
    public DbSet<SkillsCategory> SkillsCategories { get; set; } // تأكد من الاسم (SkillsCategory vs SkillCategory)
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<TopicInterest> TopicInterests { get; set; }

    // 📦 ProfileContext
    // Profile is defined in SocialGraph as well, assuming shared reference or mapping
    public DbSet<ProfileInterest> ProfileInterests { get; set; }
    public DbSet<ProfileInterestRequest> ProfileInterestRequests { get; set; }
    public DbSet<ProfileSkill> ProfileSkills { get; set; }
    public DbSet<ProfileSkillRequest> ProfileSkillRequests { get; set; }
    public DbSet<ProfileTopic> ProfileTopics { get; set; }

    // 📦 Projects
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectMedia> ProjectMedia { get; set; }
    public DbSet<ProjectMilestone> ProjectMilestones { get; set; }
    // Bridges
    public DbSet<ProjectContributor> ProjectContributors { get; set; }
    public DbSet<ProjectRating> ProjectRatings { get; set; }
    public DbSet<ProjectSkill> ProjectSkills { get; set; }
    public DbSet<ProjectTag> ProjectTags { get; set; }
    public DbSet<ProjectView> ProjectViews { get; set; }

    // 📦 QA
    public DbSet<Discussion> Discussions { get; set; }
    public DbSet<Problem> Problems { get; set; }
    public DbSet<ProblemContentBlock> ProblemContentBlocks { get; set; }
    public DbSet<Solution> Solutions { get; set; }
    public DbSet<SolutionContentBlock> SolutionContentBlocks { get; set; }
    // Bridges
    public DbSet<ProblemTag> ProblemTags { get; set; }
    public DbSet<ProblemTopic> ProblemTopics { get; set; }
    public DbSet<ProblemView> ProblemViews { get; set; }
    public DbSet<ProblemVote> ProblemVotes { get; set; }
    public DbSet<SolutionVote> SolutionVotes { get; set; }

    // 📦 Resumes
    public DbSet<Resume> Resumes { get; set; }
    public DbSet<ResumeCertificate> ResumeCertificates { get; set; }
    public DbSet<ResumeEducation> ResumeEducations { get; set; }
    public DbSet<ResumeExperience> ResumeExperiences { get; set; }
    public DbSet<ResumeLanguage> ResumeLanguages { get; set; }
    public DbSet<ResumeProject> ResumeProjects { get; set; }
    // Bridges
    public DbSet<ResumeSkill> ResumeSkills { get; set; }

    // 📦 Security
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<UserArchive> UserArchives { get; set; }
    public DbSet<IdentityArchive> IdentityArchives { get; set; }
    public DbSet<PasswordArchive> PasswordArchives { get; set; }
    public DbSet<PendingUpdate> PendingUpdates { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<SupportTicket> SupportTickets { get; set; }
    public DbSet<SupportResponse> SupportResponses { get; set; }
    public DbSet<ManualRecoveryRequest> ManualRecoveryRequests { get; set; }

    // 📦 SocialGraph
    public DbSet<Profile> Profiles { get; set; }
    // Bridges
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Block> Blocks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // This single line automatically scans the current assembly (Infrastructure)
        // and applies every class that implements IEntityTypeConfiguration<T>.
        // This includes all the Post, Job, Education, and Community configurations we created.
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}