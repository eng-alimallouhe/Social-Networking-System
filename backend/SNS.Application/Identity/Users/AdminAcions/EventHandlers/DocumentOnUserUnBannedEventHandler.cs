using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Identity.Users.UsersManagement.EventHandlers.UserActivatedEvent;
using SNS.Application.Search.ContentManagement.Posts.Abstractions;
using SNS.Application.Search.Discussions.Problems.Abstractions;
using SNS.Application.Search.Identity.Users.Abstractions;
using SNS.Application.Search.Profiles.Profiles.Abstractions;
using SNS.Application.Search.Projects.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Application.Shared.Settings;
using SNS.Domain.Identity.Users.Events;
using SNS.Domain.Projects.Enums;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Identity.Users.AdminAcions.EventHandlers;

internal class DocumentOnUserUnBannedEventHandler :
    INotificationHandler<DomainEventNotification<UserUnBannedEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPostSearchService _postSearchService;
    private readonly IProblemSearchService _problemSearchService;
    private readonly IProfileSearchService _profileSearchService;
    private readonly IUserSearchService _userSearchService;
    private readonly IProjectSearchService _projectSearchService;
    private readonly IAppLogger<DocumentOnUserUnBannedEventHandler> _logger;
    private readonly ProfileSettings _profileSettings;

    public DocumentOnUserUnBannedEventHandler(
        IApplicationDbContext dbContext,
        IPostSearchService postSearchService,
        IProblemSearchService problemSearchService,
        IProfileSearchService profileSearchService,
        IUserSearchService userSearchService,
        IProjectSearchService projectSearchService,
        IAppLogger<DocumentOnUserUnBannedEventHandler> logger,
        IOptions<ProfileSettings> options)
    {
        _dbContext = dbContext;
        _postSearchService = postSearchService;
        _problemSearchService = problemSearchService;
        _profileSearchService = profileSearchService;
        _userSearchService = userSearchService;
        _projectSearchService = projectSearchService;
        _logger = logger;
        _profileSettings = options.Value;
    }

    public async Task Handle(DomainEventNotification<UserUnBannedEvent> notification, CancellationToken cancellationToken)

    {
        var userId = notification.DomainEvent.UserId;

        var profileDocument = await _dbContext.Profiles
            .AsNoTracking()
            .Where(p => p.UserId == userId)
            .Select(p => new
            {
                BaseDoc = new ProfileDocument
                {
                    Id = p.Id,
                    UserId = p.UserId,
                    FullName = p.FullName,
                    Bio = p.Bio,
                    ProfilePictureUrl = p.ProfilePictureObjectKey,
                    Specialization = p.Specialization,
                    Universities = p.AcademicRecords.Select(ar => ar.University.Name).ToList(),
                    Skills = p.ProfileSkills.Select(ps => ps.Skill.Name).ToList(),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    FollowersCount = p.Followers.Count(),
                    FollowingsCount = p.Followings.Count(),
                    BlackList = p.BlackList.Select(bl => bl.BlockedId).ToList(),
                    Reputation = p.Reputation
                },
                FirstAcademic = p.AcademicRecords
                    .Select(ar => new AcademicRecordDocument
                    {
                        UniversityName = ar.University.Name,
                        FieldOfStudy = ar.FieldOfStudy
                    }).FirstOrDefault()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (profileDocument == null)
        {
            _logger.LogWarning("Can't Find Profile for User Indexing: {UserId}", userId);
            return;
        }

        // دمج السجل الأكاديمي المستخلص ذكياً داخل المستند الأساسي
        var finalProfileDoc = profileDocument.BaseDoc;
        finalProfileDoc.AcademicRecordDocument = profileDocument.FirstAcademic ?? new AcademicRecordDocument { UniversityName = string.Empty, FieldOfStudy = string.Empty };

        // 2️⃣ جلب مستند اليوزر وتثبيت تعديلات نظام الـ Email والتوجيه المحدث
        var userDocument = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new UserDocument()
            {
                Id = u.Id,
                UserName = u.UserName,
                PreferredLanguage = u.PreferredLanguage,
                Role = u.Role.Type.ToString(),
                Email = u.Email,
                Status = u.Status,
                IsVerified = u.IsVerified,
                FailedLoginAttempts = u.FailedLoginAttempts,
                CreatedAt = u.CreatedAt,
                LastLogin = u.LastLogIn,
                IsMfaEnabled = u.UserSecuritySettings.IsMfaEnabled,
                DefaultCommunicationMethod = u.UserSecuritySettings.DefaultCommunicationMethod
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (userDocument == null)
        {
            _logger.LogWarning("Can't Find User for UserId Indexing: {UserId}", userId);
            return;
        }

        // 3️⃣ جلب المنشورات النشطة حصرًا وحمايتها دفاعيًا من تسريب المحذوفات طوعًا
        var postDocuments = await _dbContext.Posts
            .AsNoTracking()
            .Where(post => post.AuthorId == finalProfileDoc.Id && post.IsActive) // 🛡️ أمان: منع إحياء المواد المحذوفة
            .Select(p => new PostDocument
            {
                Id = p.Id,
                AuthorId = p.AuthorId,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                ReactionsCount = p.Reactions.Count(),
                CommentsCount = p.Comments.Count()
            })
            .ToListAsync(cancellationToken);

        // 4️⃣ جلب المشاكل البرمجية النشطة فقط وحمايتها أمنيًا
        var problemDocuments = await _dbContext.Problems
            .AsNoTracking()
            .Where(problem => problem.AuthorId == finalProfileDoc.Id && problem.IsActive) // 🛡️ تأمين الداتا
            .Select(pr => new ProblemDocument
            {
                Id = pr.Id,
                AuthorId = pr.AuthorId,
                AuthorName = finalProfileDoc.FullName,
                AuthorProfilePictureUrl = finalProfileDoc.ProfilePictureUrl ?? _profileSettings.DefaultProfilePictureUrl,
                AuthorSpecialization = finalProfileDoc.Specialization ?? _profileSettings.DefaultSpecialization,
                Title = pr.Title,
                Status = pr.Status,
                CommunityId = pr.CommunityId,
                CommunityLogoUrl = pr.Community != null ? pr.Community.LogoObjectKey : null,
                CreatedAt = pr.CreatedAt,
                UpdatedAt = pr.UpdatedAt,
                SolutionsCount = pr.Solutions.Count()
            })
            .ToListAsync(cancellationToken);

        // 5️⃣ جلب المشاريع النشطة هندسيًا وترشيد استعلامات الـ Contributors والمقاييس
        var projectDocuments = await _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.OwnerId == finalProfileDoc.Id && p.IsActive) // 🛡️ حظر المواد المحذوفة سلفًا
            .Select(pr => new ProjectDocument
            {
                Id = pr.Id,
                OwnerId = pr.OwnerId,
                Title = pr.Title,
                ShortDescription = pr.ShortDescription,
                GitHubUrl = pr.GitHubUrl,
                LiveDemoUrl = pr.LiveDemoUrl,
                ReadmeContent = pr.ReadmeContent,
                Type = pr.Type,
                Status = pr.Status,
                PublishedAt = pr.PublishedAt,
                CreatedAt = pr.CreatedAt,
                UpdatedAt = pr.UpdatedAt,
                TopThreeSkills = pr.Skills.Take(3).Select(ps => ps.Skill.Name).ToList(),
                SkillsCount = pr.Skills.Count(),
                TopThreeContributors = pr.Contributors
                    .Where(c => c.InvitingStatus == InvitingStatus.Accepted)
                    .OrderBy(c => c.RespondedAt)
                    .Take(3)
                    .Select(c => new ProjectContributorDocument
                    {
                        Id = c.Id,
                        ContributorProfilePictureUrl = c.Contributor.ProfilePictureObjectKey ?? _profileSettings.DefaultProfilePictureUrl,
                        ContributorFullName = c.Contributor.FullName
                    }).ToList(),
                ContributorsCount = pr.Contributors.Count(),
                Rate = pr.Ratings.Any() ? (decimal)pr.Ratings.Average(r => r.RatingValue) : 0, // استخدام Any لحماية الأداء
                SavesCount = pr.Saves.Count(),
                totalRates = pr.Ratings.Count()
            })
            .ToListAsync(cancellationToken);

        // دمج الاسم الكامل المحدث بالوثيقة
        userDocument.FullName = finalProfileDoc.FullName;

        // 🚀 إطلاق المايسترو المتوازي لضخ كافة المستندات لحظة واحدة داخل محرك البحث الموزع ⚡
        var profileTask = _profileSearchService.UpsertProfileAsync(finalProfileDoc, cancellationToken);
        var userTask = _userSearchService.UpsertUserAsync(userDocument, cancellationToken);
        var postTask = _postSearchService.BulkPostsAsync(postDocuments, cancellationToken);
        var problemTask = _problemSearchService.BulkProblemsAsync(problemDocuments, cancellationToken);
        var projectTask = _projectSearchService.BulkProjectsAsync(projectDocuments, cancellationToken);

        await Task.WhenAll(profileTask, userTask, postTask, problemTask, projectTask);

        _logger.LogInformation("Successfully Sync and Re-indexed All Documents for Activated User: {UserId}", userId);
    }
}
