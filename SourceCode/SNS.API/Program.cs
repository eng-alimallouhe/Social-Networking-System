using Microsoft.EntityFrameworkCore;
using SNS.Domain.Abstractions.Repositories;
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
using SNS.Domain.SocialGraph;
using SNS.Domain.SocialGraph.Bridges;
using SNS.Infrastructure.Data;
using SNS.Infrastructure.Repositories;
using SNS.Infrastructure.Repositories.Communities;
using SNS.Infrastructure.Repositories.Education;
using SNS.Infrastructure.Repositories.Jobs;
using SNS.Infrastructure.Repositories.Posts;
using SNS.Infrastructure.Repositories.Preferences;
using SNS.Infrastructure.Repositories.ProfileContext;
using SNS.Infrastructure.Repositories.QA;
using SNS.Infrastructure.Repositories.Security;
using SNS.Infrastructure.Repositories.SocialGraph;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();


builder.Services.AddDbContext<SNSDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


// ==============================================================================
// Security (Identity & System)
// ==============================================================================
// Soft Delete
builder.Services.AddScoped<ISoftDeletableRepository<User>, UserRepository>();
builder.Services.AddScoped<ISoftDeletableRepository<Role>, RoleRepository>();
builder.Services.AddScoped<ISoftDeletableRepository<SupportTicket>, SupportTicketRepository>();

// Hard Delete
builder.Services.AddScoped<IRepository<RefreshToken>, RefreshTokenRepository>();
builder.Services.AddScoped<IRepository<VerificationCode>, VerificationCodeRepository>();
builder.Services.AddScoped<IRepository<UserSession>, UserSessionRepository>();
builder.Services.AddScoped<IRepository<UserArchive>, UserArchiveRepository>();
builder.Services.AddScoped<IRepository<IdentityArchive>, IdentityArchiveRepository>();
builder.Services.AddScoped<IRepository<PasswordArchive>, PasswordArchiveRepository>();
builder.Services.AddScoped<IRepository<PendingUpdate>, PendingUpdateRepository>();
builder.Services.AddScoped<IRepository<Notification>, NotificationRepository>();
builder.Services.AddScoped<IRepository<SupportResponse>, SupportResponseRepository>();
builder.Services.AddScoped<IRepository<ManualRecoveryRequest>, ManualRecoveryRequestRepository>();


// ==============================================================================
// SocialGraph
// ==============================================================================
// Soft Delete
builder.Services.AddScoped<ISoftDeletableRepository<Profile>, ProfileRepository>();

// Hard Delete
builder.Services.AddScoped<IRepository<Follow>, FollowRepository>();
builder.Services.AddScoped<IRepository<Block>, BlockRepository>();


// ==============================================================================
// Communities
// ==============================================================================
// Soft Delete
builder.Services.AddScoped<ISoftDeletableRepository<Community>, CommunityRepository>();

// Hard Delete
builder.Services.AddScoped<IRepository<CommunityAuditLog>, CommunityAuditLogRepository>();
builder.Services.AddScoped<IRepository<CommunityCreationRequest>, CommunityCreationRequestRepository>();
builder.Services.AddScoped<IRepository<CommunityInvitation>, CommunityInvitationRepository>();
builder.Services.AddScoped<IRepository<CommunityJoinRequest>, CommunityJoinRequestRepository>();
builder.Services.AddScoped<IRepository<CommunityMembership>, CommunityMembershipRepository>();
builder.Services.AddScoped<IRepository<CommunityRule>, CommunityRuleRepository>();
builder.Services.AddScoped<IRepository<CommunitySettings>, CommunitySettingsRepository>();


// ==============================================================================
// Education
// ==============================================================================
// Soft Delete
builder.Services.AddScoped<ISoftDeletableRepository<University>, UniversityRepository>();
builder.Services.AddScoped<ISoftDeletableRepository<Faculty>, FacultyRepository>();

// Hard Delete
builder.Services.AddScoped<IRepository<FacultyRequest>, FacultyRequestRepository>();
builder.Services.AddScoped<IRepository<UniversityRequest>, UniversityRequestRepository>();
builder.Services.AddScoped<IRepository<ProfileFacultyRequest>, ProfileFacultyRequestRepository>();
builder.Services.AddScoped<IRepository<ProfileUniversityRequest>, ProfileUniversityRequestRepository>();


// ==============================================================================
// Jobs
// ==============================================================================
// Soft Delete
builder.Services.AddScoped<ISoftDeletableRepository<Job>, JobRepository>();
builder.Services.AddScoped<ISoftDeletableRepository<JobApplication>, JobApplicationRepository>();

// Hard Delete
builder.Services.AddScoped<IRepository<JobSkill>, JobSkillRepository>();


// ==============================================================================
// Posts
// ==============================================================================
// Soft Delete
builder.Services.AddScoped<ISoftDeletableRepository<Post>, PostRepository>();
builder.Services.AddScoped<ISoftDeletableRepository<Comment>, CommentRepository>();

// Hard Delete
builder.Services.AddScoped<IRepository<CommentReaction>, CommentReactionRepository>();
builder.Services.AddScoped<IRepository<PostMedia>, PostMediaRepository>();
builder.Services.AddScoped<IRepository<PostReaction>, PostReactionRepository>();
builder.Services.AddScoped<IRepository<PostTag>, PostTagRepository>();
builder.Services.AddScoped<IRepository<PostTopic>, PostTopicRepository>();
builder.Services.AddScoped<IRepository<PostView>, PostViewRepository>();


// ==============================================================================
// Preferences
// ==============================================================================
// Soft Delete
builder.Services.AddScoped<ISoftDeletableRepository<Interest>, InterestRepository>();
builder.Services.AddScoped<ISoftDeletableRepository<InterestCategory>, InterestCategoryRepository>();
builder.Services.AddScoped<ISoftDeletableRepository<Skill>, SkillRepository>();
builder.Services.AddScoped<ISoftDeletableRepository<SkillsCategory>, SkillsCategoryRepository>();

// Hard Delete
builder.Services.AddScoped<IRepository<Tag>, TagRepository>();
builder.Services.AddScoped<IRepository<Topic>, TopicRepository>();
builder.Services.AddScoped<IRepository<InterestRequest>, InterestRequestRepository>();
builder.Services.AddScoped<IRepository<SkillRequest>, SkillRequestRepository>();
builder.Services.AddScoped<IRepository<TopicInterest>, TopicInterestRepository>();


// ==============================================================================
// ProfileContext (Bridges)
// ==============================================================================
// Hard Delete
builder.Services.AddScoped<IRepository<ProfileInterest>, ProfileInterestRepository>();
builder.Services.AddScoped<IRepository<ProfileInterestRequest>, ProfileInterestRequestRepository>();
builder.Services.AddScoped<IRepository<ProfileSkill>, ProfileSkillRepository>();
builder.Services.AddScoped<IRepository<ProfileSkillRequest>, ProfileSkillRequestRepository>();
builder.Services.AddScoped<IRepository<ProfileTopic>, ProfileTopicRepository>();


// ==============================================================================
// Projects
// ==============================================================================
// Soft Delete
builder.Services.AddScoped<ISoftDeletableRepository<Project>, ProjectRepository>();

// Hard Delete
builder.Services.AddScoped<IRepository<ProjectMedia>, ProjectMediaRepository>();
builder.Services.AddScoped<IRepository<ProjectMilestone>, ProjectMilestoneRepository>();
builder.Services.AddScoped<IRepository<ProjectContributor>, ProjectContributorRepository>();
builder.Services.AddScoped<IRepository<ProjectRating>, ProjectRatingRepository>();
builder.Services.AddScoped<IRepository<ProjectSkill>, ProjectSkillRepository>();
builder.Services.AddScoped<IRepository<ProjectTag>, ProjectTagRepository>();
builder.Services.AddScoped<IRepository<ProjectView>, ProjectViewRepository>();


// ==============================================================================
// QA (Questions & Answers)
// ==============================================================================
// Soft Delete
builder.Services.AddScoped<ISoftDeletableRepository<Discussion>, DiscussionRepository>();
builder.Services.AddScoped<ISoftDeletableRepository<Problem>, ProblemRepository>();
builder.Services.AddScoped<ISoftDeletableRepository<Solution>, SolutionRepository>();

// Hard Delete
builder.Services.AddScoped<IRepository<ProblemContentBlock>, ProblemContentBlockRepository>();
builder.Services.AddScoped<IRepository<SolutionContentBlock>, SolutionContentBlockRepository>();
builder.Services.AddScoped<IRepository<ProblemTag>, ProblemTagRepository>();
builder.Services.AddScoped<IRepository<ProblemTopic>, ProblemTopicRepository>();
builder.Services.AddScoped<IRepository<ProblemView>, ProblemViewRepository>();
builder.Services.AddScoped<IRepository<ProblemVote>, ProblemVoteRepository>();
builder.Services.AddScoped<IRepository<SolutionVote>, SolutionVoteRepository>();


// ==============================================================================
// Resumes
// ==============================================================================
// Soft Delete
builder.Services.AddScoped<ISoftDeletableRepository<Resume>, ResumeRepository>();

// Hard Delete
builder.Services.AddScoped<IRepository<ResumeCertificate>, ResumeCertificateRepository>();
builder.Services.AddScoped<IRepository<ResumeEducation>, ResumeEducationRepository>();
builder.Services.AddScoped<IRepository<ResumeExperience>, ResumeExperienceRepository>();
builder.Services.AddScoped<IRepository<ResumeLanguage>, ResumeLanguageRepository>();
builder.Services.AddScoped<IRepository<ResumeProject>, ResumeProjectRepository>();
builder.Services.AddScoped<IRepository<ResumeSkill>, ResumeSkillRepository>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
