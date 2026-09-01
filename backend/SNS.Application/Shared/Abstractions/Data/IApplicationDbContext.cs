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
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Relations;
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

namespace SNS.Application.Shared.Abstractions.Data;

public interface IApplicationDbContext
{
    // 📦 Communities
    IQueryable<Community> Communities { get; }
    IQueryable<CommunityAuditLog> CommunityAuditLogs { get; }
    IQueryable<CommunityInvitation> CommunityInvitations { get; }
    IQueryable<CommunityJoinRequest> CommunityJoinRequests { get; }
    IQueryable<CommunityMembership> CommunityMemberships { get; }
    IQueryable<CommunityRule> CommunityRules { get; }
    IQueryable<CommunitySettings> CommunitySettings { get; }


    // 📦 Educations
    IQueryable<AcademicRecord> AcademicRecords { get; }
    IQueryable<University> Universities { get; }

    // 📦 Jobs
    IQueryable<Job> Jobs { get; }
    IQueryable<JobApplication> JobApplications { get; }
    IQueryable<JobSkill> JobSkills { get; }
    IQueryable<Company> Companies { get; }
    IQueryable<CompanyAdministrator> CompanyAdministrators { get; }
    IQueryable<SavedJob> SavedJobs { get; }
    IQueryable<CompanyCreateRequest> CompanyCreateRequests { get; }


    // 📦 ContentManagement
    IQueryable<Post> Posts { get; }
    IQueryable<Comment> Comments { get; }
    IQueryable<CommentReaction> CommentReactions { get; }
    IQueryable<PostMedia> PostMedia { get; }
    IQueryable<PostReaction> PostReactions { get; }
    IQueryable<CommentMention> CommentMentions { get; }
    IQueryable<SavedPost> SavedPosts { get; }

    
    // Bridges
    IQueryable<PostTag> PostTags { get; }
    IQueryable<PostTopic> PostTopics { get; }
    IQueryable<PostView> PostViews { get; }

    // 📦 Preferences

    IQueryable<Skill> Skills { get; }
    IQueryable<SkillsCategory> SkillsCategories { get; }
    IQueryable<Tag> Tags { get; }
    IQueryable<Topic> Topics { get; }


    // 📦 ProfileContext

    IQueryable<ProfileSkill> ProfileSkills { get; }
    IQueryable<ProfileTopic> ProfileTopics { get; }

    // 📦 Projects
    IQueryable<Project> Projects { get; }
    IQueryable<ProjectMedia> ProjectMedia { get; }
    IQueryable<ProjectMilestone> ProjectMilestones { get; }
    // Bridges
    IQueryable<ProjectContributor> ProjectContributors { get; }
    IQueryable<ProjectRating> ProjectRatings { get; }
    IQueryable<ProjectSkill> ProjectSkills { get; }
    IQueryable<ProjectTag> ProjectTags { get; }
    IQueryable<ProjectView> ProjectViews { get; }
    IQueryable<SavedProject> SavedProjects { get; }



    // 📦 QA
    IQueryable<Discussion> Discussions { get; }
    IQueryable<Problem> Problems { get; }
    IQueryable<ProblemContentBlock> ProblemContentBlocks { get; }
    IQueryable<Solution> Solutions { get; }
    IQueryable<SavedProblem> SavedProblems { get; }
    IQueryable<SavedSolution> SavedSolutions { get; }
    IQueryable<SolutionContentBlock> SolutionContentBlocks { get; }


    // Bridges
    IQueryable<ProblemTag> ProblemTags { get; }
    IQueryable<ProblemTopic> ProblemTopics { get; }
    IQueryable<ProblemView> ProblemViews { get; }
    IQueryable<ProblemVote> ProblemVotes { get; }
    IQueryable<SolutionVote> SolutionVotes { get; }

    // 📦 Resumes
    IQueryable<Resume> Resumes { get; }
    IQueryable<ResumeCertificate> ResumeCertificates { get; }
    IQueryable<ResumeEducation> ResumeEducations { get; }
    IQueryable<ResumeExperience> ResumeExperiences { get; }
    IQueryable<ResumeLanguage> ResumeLanguages { get; }
    IQueryable<ResumeProject> ResumeProjects { get; }
    // Bridges
    IQueryable<ResumeSkill> ResumeSkills { get; }

    // 📦 Security
    IQueryable<User> Users { get; }
    IQueryable<Role> Roles { get; }
    IQueryable<Permission> Permissions { get; }
    IQueryable<RolePermission> RolePermissions { get; }
    IQueryable<Device> Devices { get; }
    IQueryable<SecuritySession> UserSessions { get; }
    IQueryable<UserArchive> UserArchives { get; }
    IQueryable<IdentityArchive> IdentityArchives { get; }
    IQueryable<PasswordArchive> PasswordArchives { get; }
    IQueryable<Notification> Notifications { get; }
    IQueryable<UserNotificationPreferences> UserNotificationPreferences { get; }
    IQueryable<UserSecuritySettings> UsersSecuritySettings { get; }
    IQueryable<UserPasskey> UserPasskeys { get; }
    IQueryable<ExportDataRequest> ExportDataRequests { get; }
    IQueryable<RecoveryCode> RecoveryCodes { get; }



    // 📦 Profiles
    IQueryable<Profile> Profiles { get; }
    IQueryable<ReputationLedger> ReputationLedgers { get; }
    IQueryable<SavedProfile> SavedProfiles { get; }
    IQueryable<ProfileView> ProfileViews { get; }
    IQueryable<ProfileTag> ProfileTags { get; }


    // Bridges
    IQueryable<Follow> Follows { get; }
    IQueryable<Block> Blocks { get; }

    // 📦 Moderation
    IQueryable<SNS.Domain.Moderation.Entities.ContentReport> ContentReports { get; }
    IQueryable<SNS.Domain.Moderation.Entities.ReportTicket> ReportTickets { get; }

    // 📦 Support
    IQueryable<SNS.Domain.Support.Entities.SupportTicket> SupportTickets { get; }
    IQueryable<SNS.Domain.Support.Entities.TicketMessage> TicketMessages { get; }
    IQueryable<SNS.Domain.Support.Entities.TicketMessageAttachment> TicketMessageAttachments { get; }
}
