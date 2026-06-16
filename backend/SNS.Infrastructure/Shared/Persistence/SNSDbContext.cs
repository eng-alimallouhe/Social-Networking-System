using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.ContentManagement.Comments.Entities;
using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Domain.ContentManagement.Posts.Entities;
using SNS.Domain.Discussions.Problems.Entities;
using SNS.Domain.Discussions.Problems.Relations;
using SNS.Domain.Discussions.Solutions.Entities;
using SNS.Domain.Discussions.Solutions.Relations;
using SNS.Domain.Educations.Entities;
using SNS.Domain.Identity.ArchiveManagement.Entities;
using SNS.Domain.Identity.Notifications.Entities;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Identity.Users.Constants;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Jobs.Entities;
using SNS.Domain.Jobs.Relations;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.Profiles.Profiles.Entities;
using SNS.Domain.Profiles.Profiles.Relations;
using SNS.Domain.Profiles.SocialGraph.Entities;
using SNS.Domain.Projects.Bridges;
using SNS.Domain.Projects.Entities;
using SNS.Domain.Resumes.Bridges;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Shared.Entities;
using SNS.Domain.Shared.Events;
using System.Reflection;
using System.Text.Json;

namespace SNS.Infrastructure.Persistence;

public class SNSDbContext : DbContext, IApplicationDbContext
{
    private readonly IMediator _mediator;

    public SNSDbContext(
        DbContextOptions<SNSDbContext> options,
        IMediator mediator) : base(options)
    {
        _mediator = mediator;
    }



    // 📦 Communities
    public DbSet<Community> Communities { get; set; }
    public DbSet<CommunityAuditLog> CommunityAuditLogs { get; set; }
    public DbSet<CommunityInvitation> CommunityInvitations { get; set; }
    public DbSet<CommunityJoinRequest> CommunityJoinRequests { get; set; }
    public DbSet<CommunityMembership> CommunityMemberships { get; set; }
    public DbSet<CommunityRule> CommunityRules { get; set; }
    public DbSet<CommunitySettings> CommunitySettings { get; set; }

    // 📦 Educations
    public DbSet<AcademicRecord> AcademicRecords { get; set; }
    public DbSet<University> Universities { get; set; }

    // 📦 Jobs
    public DbSet<Job> Jobs { get; set; }
    public DbSet<JobApplication> JobApplications { get; set; }
    public DbSet<JobSkill> JobSkills { get; set; }
    public DbSet<Company> Companies { get; set; }
    public DbSet<CompanyAdministrator> CompanyAdministrators { get; set; }
    public DbSet<SavedJob> SavedJobs { get; set; }

    // 📦 ContentManagement
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentReaction> CommentReactions { get; set; }
    public DbSet<PostMedia> PostMedia { get; set; }
    public DbSet<PostReaction> PostReactions { get; set; }
    public DbSet<CommentMedia> CommentMedias { get; set; }
    public DbSet<SavedPost> SavedPosts { get; set; }

    // Bridges
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<PostTopic> PostTopics { get; set; }
    public DbSet<PostView> PostViews { get; set; }

    // 📦 Preferences

    public DbSet<Skill> Skills { get; set; }
    public DbSet<SkillsCategory> SkillsCategories { get; set; } 
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Topic> Topics { get; set; }


    // 📦 ProfileContext
    // Profile is defined in Profiles as well, assuming shared reference or mapping

    public DbSet<ProfileSkill> ProfileSkills { get; set; }
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
    public DbSet<SavedProject> SavedProjects { get; set; }

    // 📦 QA
    public DbSet<Discussion> Discussions { get; set; }
    public DbSet<Problem> Problems { get; set; }
    public DbSet<ProblemContentBlock> ProblemContentBlocks { get; set; }
    public DbSet<Solution> Solutions { get; set; }
    public DbSet<SolutionContentBlock> SolutionContentBlocks { get; set; }
    public DbSet<SavedSolution> SavedSolutions { get; set; }
    public DbSet<SavedProblem> SavedProblems { get; set; }

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

    // 📦 Identity
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<SecuritySession> UserSessions { get; set; }
    public DbSet<UserArchive> UserArchives { get; set; }
    public DbSet<IdentityArchive> IdentityArchives { get; set; }
    public DbSet<PasswordArchive> PasswordArchives { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<UserNotificationPreferences> UserNotificationPreferences { get; set; }
    public DbSet<UserSecuritySettings> UsersSecuritySettings { get; set; }
    public DbSet<UserPasskey> UserPasskeys { get; set; }
    public DbSet<Device> Devices { get; set; }
    public DbSet<ExportDataRequest> ExportDataRequests { get; set; }
    public DbSet<RecoveryCode> RecoveryCodes { get; set; }


    // 📦 Profiles
    public DbSet<Profile> Profiles { get; set; }
    public DbSet<ReputationLedger> ReputationLedgers { get; set; }
    public DbSet<SavedProfile> SavedProfiles { get; set; }

    // Bridges
    public DbSet<Follow> Follows { get; set; }
    public DbSet<Block> Blocks { get; set; }

    // 📦 OutboxMessage
    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    // ====================================================================
    // IApplicationDbContext Implementation (Read-Only)
    // ====================================================================

    // 📦 Communities
    IQueryable<Community> IApplicationDbContext.Communities => Communities.AsNoTracking();
    IQueryable<CommunityAuditLog> IApplicationDbContext.CommunityAuditLogs => CommunityAuditLogs.AsNoTracking();
    IQueryable<CommunityInvitation> IApplicationDbContext.CommunityInvitations => CommunityInvitations.AsNoTracking();
    IQueryable<CommunityJoinRequest> IApplicationDbContext.CommunityJoinRequests => CommunityJoinRequests.AsNoTracking();
    IQueryable<CommunityMembership> IApplicationDbContext.CommunityMemberships => CommunityMemberships.AsNoTracking();
    IQueryable<CommunityRule> IApplicationDbContext.CommunityRules => CommunityRules.AsNoTracking();
    IQueryable<CommunitySettings> IApplicationDbContext.CommunitySettings => CommunitySettings.AsNoTracking();

    // 📦 Educations
    IQueryable<AcademicRecord> IApplicationDbContext.AcademicRecords => AcademicRecords.AsNoTracking();

    IQueryable<University> IApplicationDbContext.Universities => Universities.AsNoTracking();

    // 📦 Jobs
    IQueryable<Job> IApplicationDbContext.Jobs => Jobs.AsNoTracking();
    IQueryable<JobApplication> IApplicationDbContext.JobApplications => JobApplications.AsNoTracking();
    IQueryable<JobSkill> IApplicationDbContext.JobSkills => JobSkills.AsNoTracking();
    IQueryable<Company> IApplicationDbContext.Companies => Companies.AsNoTracking();
    IQueryable<CompanyAdministrator> IApplicationDbContext.CompanyAdministrators => CompanyAdministrators.AsNoTracking();

    IQueryable<SavedJob> IApplicationDbContext.SavedJobs => SavedJobs.AsNoTracking();

    // 📦 ContentManagement
    IQueryable<Post> IApplicationDbContext.Posts => Posts.AsNoTracking();
    IQueryable<Comment> IApplicationDbContext.Comments => Comments.AsNoTracking();
    IQueryable<CommentReaction> IApplicationDbContext.CommentReactions => CommentReactions.AsNoTracking();
    IQueryable<PostMedia> IApplicationDbContext.PostMedia => PostMedia.AsNoTracking();
    IQueryable<PostReaction> IApplicationDbContext.PostReactions => PostReactions.AsNoTracking();
    IQueryable<CommentMedia> IApplicationDbContext.CommentMedias => CommentMedias.AsNoTracking();
    IQueryable<SavedPost> IApplicationDbContext.SavedPosts => SavedPosts.AsNoTracking();

    // Bridges
    IQueryable<PostTag> IApplicationDbContext.PostTags => PostTags.AsNoTracking();
    IQueryable<PostTopic> IApplicationDbContext.PostTopics => PostTopics.AsNoTracking();
    IQueryable<PostView> IApplicationDbContext.PostViews => PostViews.AsNoTracking();

    // 📦 Preferences

    IQueryable<Skill> IApplicationDbContext.Skills => Skills.AsNoTracking();
    IQueryable<SkillsCategory> IApplicationDbContext.SkillsCategories => SkillsCategories.AsNoTracking();
    IQueryable<Tag> IApplicationDbContext.Tags => Tags.AsNoTracking();
    IQueryable<Topic> IApplicationDbContext.Topics => Topics.AsNoTracking();


    // 📦 ProfileContext

    IQueryable<ProfileSkill> IApplicationDbContext.ProfileSkills => ProfileSkills.AsNoTracking();
    IQueryable<ProfileTopic> IApplicationDbContext.ProfileTopics => ProfileTopics.AsNoTracking();

    // 📦 Projects
    IQueryable<Project> IApplicationDbContext.Projects => Projects.AsNoTracking();
    IQueryable<ProjectMedia> IApplicationDbContext.ProjectMedia => ProjectMedia.AsNoTracking();
    IQueryable<ProjectMilestone> IApplicationDbContext.ProjectMilestones => ProjectMilestones.AsNoTracking();
    // Bridges
    IQueryable<ProjectContributor> IApplicationDbContext.ProjectContributors => ProjectContributors.AsNoTracking();
    IQueryable<ProjectRating> IApplicationDbContext.ProjectRatings => ProjectRatings.AsNoTracking();
    IQueryable<ProjectSkill> IApplicationDbContext.ProjectSkills => ProjectSkills.AsNoTracking();
    IQueryable<ProjectTag> IApplicationDbContext.ProjectTags => ProjectTags.AsNoTracking();
    IQueryable<ProjectView> IApplicationDbContext.ProjectViews => ProjectViews.AsNoTracking();

    IQueryable<SavedProject> IApplicationDbContext.SavedProjects => SavedProjects.AsNoTracking();

    // 📦 QA
    IQueryable<Discussion> IApplicationDbContext.Discussions => Discussions.AsNoTracking();
    IQueryable<Problem> IApplicationDbContext.Problems => Problems.AsNoTracking();
    IQueryable<ProblemContentBlock> IApplicationDbContext.ProblemContentBlocks => ProblemContentBlocks.AsNoTracking();
    IQueryable<Solution> IApplicationDbContext.Solutions => Solutions.AsNoTracking();
    IQueryable<SolutionContentBlock> IApplicationDbContext.SolutionContentBlocks => SolutionContentBlocks.AsNoTracking();
    // Bridges
    IQueryable<ProblemTag> IApplicationDbContext.ProblemTags => ProblemTags.AsNoTracking();
    IQueryable<ProblemTopic> IApplicationDbContext.ProblemTopics => ProblemTopics.AsNoTracking();
    IQueryable<ProblemView> IApplicationDbContext.ProblemViews => ProblemViews.AsNoTracking();
    IQueryable<ProblemVote> IApplicationDbContext.ProblemVotes => ProblemVotes.AsNoTracking();
    IQueryable<SolutionVote> IApplicationDbContext.SolutionVotes => SolutionVotes.AsNoTracking();
    IQueryable<SavedProblem> IApplicationDbContext.SavedProblems => SavedProblems.AsNoTracking();
    IQueryable<SavedSolution> IApplicationDbContext.SavedSolutions => SavedSolutions.AsNoTracking();


    // 📦 Resumes
    IQueryable<Resume> IApplicationDbContext.Resumes => Resumes.AsNoTracking();
    IQueryable<ResumeCertificate> IApplicationDbContext.ResumeCertificates => ResumeCertificates.AsNoTracking();
    IQueryable<ResumeEducation> IApplicationDbContext.ResumeEducations => ResumeEducations.AsNoTracking();
    IQueryable<ResumeExperience> IApplicationDbContext.ResumeExperiences => ResumeExperiences.AsNoTracking();
    IQueryable<ResumeLanguage> IApplicationDbContext.ResumeLanguages => ResumeLanguages.AsNoTracking();
    IQueryable<ResumeProject> IApplicationDbContext.ResumeProjects => ResumeProjects.AsNoTracking();
    
    // Bridges
    IQueryable<ResumeSkill> IApplicationDbContext.ResumeSkills => ResumeSkills.AsNoTracking();

    // 📦 Security
    IQueryable<User> IApplicationDbContext.Users => Users.AsNoTracking();
    IQueryable<Role> IApplicationDbContext.Roles => Roles.AsNoTracking();
    IQueryable<RefreshToken> IApplicationDbContext.RefreshTokens => RefreshTokens.AsNoTracking();
    IQueryable<SecuritySession> IApplicationDbContext.UserSessions => UserSessions.AsNoTracking();
    IQueryable<UserArchive> IApplicationDbContext.UserArchives => UserArchives.AsNoTracking();
    IQueryable<IdentityArchive> IApplicationDbContext.IdentityArchives => IdentityArchives.AsNoTracking();
    IQueryable<PasswordArchive> IApplicationDbContext.PasswordArchives => PasswordArchives.AsNoTracking();
    IQueryable<Notification> IApplicationDbContext.Notifications => Notifications.AsNoTracking();
    IQueryable<UserNotificationPreferences> IApplicationDbContext.UserNotificationPreferences => UserNotificationPreferences.AsNoTracking();
    IQueryable<UserSecuritySettings> IApplicationDbContext.UsersSecuritySettings => UsersSecuritySettings.AsNoTracking();
    IQueryable<UserPasskey> IApplicationDbContext.UserPasskeys => UserPasskeys.AsNoTracking();
    IQueryable<Device> IApplicationDbContext.Devices => Devices.AsNoTracking();
    IQueryable<ExportDataRequest> IApplicationDbContext.ExportDataRequests => ExportDataRequests.AsNoTracking();
    IQueryable<RecoveryCode> IApplicationDbContext.RecoveryCodes => RecoveryCodes.AsNoTracking();

    // 📦 Profiles
    IQueryable<Profile> IApplicationDbContext.Profiles => Profiles.AsNoTracking();
    IQueryable<ReputationLedger> IApplicationDbContext.ReputationLedgers => ReputationLedgers.AsNoTracking();
    // Bridges
    IQueryable<Follow> IApplicationDbContext.Follows => Follows.AsNoTracking();
    IQueryable<Block> IApplicationDbContext.Blocks => Blocks.AsNoTracking();
    IQueryable<SavedProfile> IApplicationDbContext.SavedProfiles => SavedProfiles.AsNoTracking();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Role>().HasData(
            Role.CreateSystemUserRole(SystemUsers.GhostRoleId));

        modelBuilder.Entity<User>().HasData(User.CreateSystemUser(
            SystemUsers.GhostUserId, 
            SystemUsers.GhostRoleId,
            "deleted_user", 
            "deleted_user@system.sns", 
            SystemUsers.GhostUserPassword));

        modelBuilder.Entity<Profile>().HasData(Profile.CreateSystemProfile(
            SystemUsers.GhostProfileId,
            SystemUsers.GhostUserId, 
            "Deleted User",
            SystemUsers.GhostProfilePicture));

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entitiesWithEvents = ChangeTracker.Entries<Entity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var domainEvents = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        var synchronousEvents = domainEvents.Where(
            de => de.EventType == Domain.Shared.Events.EventType.Synchronous).ToList();

        var integrationEvents = domainEvents.Where(
            de => de.EventType == Domain.Shared.Events.EventType.Integration).ToList();

        entitiesWithEvents.ForEach(e => e.ClearDomainEvents());

        var outboxMessages = integrationEvents.Select(domainEvent => {
            var msg = OutboxMessage.Create(
                type: domainEvent.GetType().AssemblyQualifiedName ?? domainEvent.GetType().Name,
                content: JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
            );
            return msg;
        }).ToList();

        if (outboxMessages.Any())
        {
            this.Set<OutboxMessage>().AddRange(outboxMessages);
        }

        if (synchronousEvents.Any())
        {
            foreach (var synchronousEvent in synchronousEvents)
            {
                var notification = CreateDomainEventNotification(synchronousEvent);
                await _mediator.Publish(notification);
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    private INotification CreateDomainEventNotification(IDomainEvent domainEvent)
    {
        var genericDispatcherType = typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType());

        return (INotification)Activator.CreateInstance(genericDispatcherType, domainEvent)!;
    }
}
