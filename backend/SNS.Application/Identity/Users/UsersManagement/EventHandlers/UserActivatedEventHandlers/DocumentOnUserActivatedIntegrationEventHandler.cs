using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SNS.Application.Abstractions.Loggings;
using SNS.Application.Search.ContentManagement.Posts.Abstractions;
using SNS.Application.Search.Identity.Users.Abstractions;
using SNS.Application.Search.Profiles.Profiles.Abstractions;
using SNS.Application.Search.Projects.Abstractions;
using SNS.Application.Search.Discussions.Problems.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Application.Shared.Settings;
using SNS.Domain.Identity.Users.Events;
using SNS.Domain.Projects.Enums;
using SNS.Domain.Search.Documents;

namespace SNS.Application.Identity.Users.UsersManagement.EventHandlers.UserActivatedEvent;

public class DocumentOnUserActivatedIntegrationEventHandler : INotificationHandler<DomainEventNotification<UserActivatedIntegrationEvent>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IPostSearchService _postSearchService;
    private readonly IProblemSearchService _problemSearchService;
    private readonly IProfileSearchService _profileSearchService;
    private readonly IUserSearchService _userSearchService;
    private readonly IProjectSearchService _projectSearchService;
    private readonly IAppLogger<DocumentOnUserActivatedIntegrationEventHandler> _logger;
    private readonly ProfileSettings _profileSettings;

    public DocumentOnUserActivatedIntegrationEventHandler(
        IApplicationDbContext dbContext,
        IPostSearchService postSearchService,
        IProblemSearchService problemSearchService,
        IProfileSearchService profileSearchService,
        IUserSearchService userSearchService,
        IProjectSearchService projectSearchService,
        IAppLogger<DocumentOnUserActivatedIntegrationEventHandler> logger,
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

    public async Task Handle(DomainEventNotification<UserActivatedIntegrationEvent> notification, CancellationToken cancellationToken)
    {
        var userId = notification.DomainEvent.UserId;

        var finalProfileDoc = await _dbContext.Profiles
            .AsNoTracking() 
            .Where(p => p.UserId == userId)
            .Select(p => new ProfileDocument
            {
                Id = p.Id,
                FullName = p.FullName,
                Bio = p.Bio,
                Specialization = p.Specialization,
                Universities = p.AcademicRecords.Select(ar => ar.University.Name).ToList(),
                Skills = p.ProfileSkills.Where(ps => ps.ProfileId == p.Id).Select(ps => ps.Skill.Name).ToList(),
                CreatedAt = p.CreatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (finalProfileDoc == null)
        {
            _logger.LogWarning("Can't Find Profile for User Indexing: {UserId}", userId);
            return;
        }

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
                FullName = u.UserProfile.FullName,
                Email = u.Email,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
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
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Topics = p.PostTopics.Select(pt => pt.Topic.Name).ToList(),
                Tags = p.PostTags.Select(pt => pt.Tag.Name).ToList()
            })
            .ToListAsync(cancellationToken);

        // 4️⃣ جلب المشاكل البرمجية النشطة فقط وحمايتها أمنيًا
        var problemDocuments = await _dbContext.Problems
            .AsNoTracking()
            .Where(problem => problem.AuthorId == finalProfileDoc.Id && problem.IsActive) // 🛡️ تأمين الداتا
            .Select(pr => new ProblemDocument
            {
                Id = pr.Id,
                Title = pr.Title,
                Status = pr.Status,
                Level = pr.Level,
                CreatedAt = pr.CreatedAt,
                UpdatedAt = pr.UpdatedAt,
                Topics = pr.ProblemTopics.Select(pt => pt.Topic.Name).ToList(),
                Tags = pr.ProblemTags.Select(pt => pt.Tag.Name).ToList()
            })
            .ToListAsync(cancellationToken);

        // 5️⃣ جلب المشاريع النشطة هندسيًا
        var projectDocuments = await _dbContext.Projects
            .AsNoTracking()
            .Where(p => p.OwnerId == finalProfileDoc.Id && p.IsActive) // 🛡️ حظر المواد المحذوفة سلفًا
            .Select(pr => new ProjectDocument
            {
                Id = pr.Id,
                Title = pr.Title,
                ShortDescription = pr.ShortDescription,
                ReadmeContent = pr.ReadmeContent,
                Type = pr.Type,
                Status = pr.Status,
                PublishedAt = pr.PublishedAt,
                CreatedAt = pr.CreatedAt,
                UpdatedAt = pr.UpdatedAt,
                Skills = pr.Skills.Select(ps => ps.Skill.Name).ToList(),
                Tags = pr.Tags.Select(pt => pt.Tag.Name).ToList()
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