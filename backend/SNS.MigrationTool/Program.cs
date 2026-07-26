using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SNS.Application;
using SNS.Application.Search.ContentManagement.Communitites.Abstractions;
using SNS.Application.Search.ContentManagement.Posts.Abstractions;
using SNS.Application.Search.Discussions.Problems.Abstractions;
using SNS.Application.Search.Identity.Users.Abstractions;
using SNS.Application.Search.Jobs.Abstractions;
using SNS.Application.Search.Profiles.Profiles.Abstractions;
using SNS.Application.Search.Projects.Abstractions;
using SNS.Domain.Discussions.Shared.Enums;
using SNS.Domain.Search.Documents;
using SNS.Infrastructure;
using SNS.Infrastructure.Persistence;

internal class Program
{
    private async static Task Main(string[] args)
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        services.AddLogging(configure => configure.AddConsole());

        services.AddSingleton(configuration);

        services.AddInfrastructureDI(configuration);
        services.AddApplicationDI(configuration);

        var serviceProvider = services.BuildServiceProvider();


        var dbContext = serviceProvider.GetRequiredService<SNSDbContext>();

        Console.WriteLine(
            BCrypt.Net.BCrypt.HashPassword(
                "alimallohi0947041713A"));

        var posts = await dbContext.Posts
            .Select(p => new PostDocument()
            {
                Id = p.Id,
                AuthorId = p.AuthorId,
                
                CommunityId = p.Community != null ? p.Id : (Guid?)null,
                

                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                LastInteractedAt = p.LastInteractedAt,

                Topics = p.PostTopics.Select(t => t.Topic.Name).ToList(),

                Tags = p.PostTags.Select(pt => pt.Tag.Name).ToList(),

                CommentsCount = p.Comments.Count,
                ReactionsCount = p.Reactions.Count,
                ViewsCount = p.Views.Count,
                SavesCount = p.SavedPosts.Count,

                MediaUrls = p.Media.Select(m => m.ObjectKey).ToList()
            })
            .ToListAsync();

        var profiles = await dbContext.Profiles
            .Select(p => new ProfileDocument()
            {
                Id = p.Id,
                UserId = p.UserId,
                FullName = p.FullName,
                Specialization = p.Specialization,
                Bio = p.Bio,
                ProfilePictureUrl = p.ProfilePictureObjectKey,
                Universities = p.AcademicRecords.Select(ar => ar.University.Name).ToList(),

                AcademicRecordDocument = new AcademicRecordDocument()
                {
                    UniversityName = p.AcademicRecords.FirstOrDefault() != null ? p.AcademicRecords.FirstOrDefault()!.University.Name : "N/A",
                    FieldOfStudy = p.AcademicRecords.FirstOrDefault() != null ? p.AcademicRecords.FirstOrDefault()!.FieldOfStudy : "N/A",
                },

                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                FollowersCount = p.Followers.Count,
                FollowingsCount = p.Followings.Count,
                Reputation = p.Reputation,
                BlackList = p.BlackList.Select(b => b.BlockedId).ToList(),
                Skills = p.ProfileSkills.Select(s => s.Skill.Name).ToList()
            })
            .ToListAsync();

        var communities = await dbContext.Communities
            .Select(c => new CommunityDocument()
            {
                Id = c.Id,
                Name = c.Name,
                LogoUrl = c.LogoObjectKey,
                Description = c.Description,
                Type = c.Type,
                CreatedAt = c.CreatedAt,
                UpdateAt = c.UpdateAt,
                MembersCount = c.Memberships.Count,
                OwnerId = c.OwnerId,
                OwnerName = c.Owner.FullName,
                OwnerProfilePicture = c.Owner.ProfilePictureObjectKey ?? "N/A"
            })
            .ToListAsync();


        var jobs = await dbContext.Jobs
            .Select(j => new JobsDocument()
            {
                Id = j.Id,
                Title = j.Title,
                Description = j.Description,
                Location = j.Location,
                Type = j.Type,
                MinSalary = j.MinSalary,
                MaxSalary = j.MaxSalary,
                CurrencyCode = j.CurrencyCode,
                SalaryType = j.SalaryType,
                CreatedAt = j.CreatedAt,
                ClosedAt = j.ClosedAt,
                CompanyId = j.CompanyId,
                CompanyName = j.Company.Name,
            })
            .ToListAsync();


        var problems = await dbContext.Problems
            .Select(p => new ProblemDocument()
            {
                Id = p.Id,
                AuthorId = p.AuthorId,
                AuthorName = p.Author.FullName,
                AuthorProfilePictureUrl = p.Author.ProfilePictureObjectKey ?? "N/A",
                AuthorSpecialization = p.Author.Specialization ?? "N/A",
                CommunityId = p.Community != null ? p.Id : (Guid?)null,
                CommunityName = p.Community != null ? p.Community.Name : "N/A",
                CommunityLogoUrl = p.Community != null ? p.Community.LogoObjectKey : "N/A",
                Title = p.Title,
                Status = p.Status,
                Level = p.Level,
                TopTwoContentBlock = p.ContentBlocks
                    .OrderBy(cb => cb.Order)
                    .Take(2)
                    .Select(cb => new ProblemBlockDocument()
                    {
                        Type = cb.Type,
                        Content = cb.Content,
                        ExtraInfo = cb.ExtraInfo,
                        Order = cb.Order
                    })
                    .ToList(),
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                IsActive = p.IsActive,
                UpVotesCount = p.Votes.Count(pv => pv.Type == VoteType.Upvote),
                DownVotesCount = p.Votes.Count(pv => pv.Type == VoteType.Downvote),
                SolutionsCount = p.Solutions.Count,
                ViewsCount = p.Views.Count,
                Tags = p.ProblemTags.Select(pt => pt.Tag.Name).ToList(),
                Topics = p.ProblemTopics.Select(pt => pt.Topic.Name).ToList()
            })
            .ToListAsync();


        var projects = await dbContext.Projects
            .Select(p => new ProjectDocument()
            {
                Id = p.Id,

                Title = p.Title,
                ShortDescription = p.ShortDescription,
                GitHubUrl = p.GitHubUrl,
                LiveDemoUrl = p.LiveDemoUrl,
                ReadmeContent = p.ReadmeContent,
                Type = p.Type,
                Status = p.Status,
                PublishedAt = p.PublishedAt,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                TopThreeSkills = p.Skills
                    .OrderByDescending(s => s.Id)
                    .Take(3)
                    .Select(s => s.Skill.Name)
                    .ToList(),
                TopThreeContributors = p.Contributors
                    .Where(p => p.InvitingStatus == SNS.Domain.Projects.Enums.InvitingStatus.Accepted)
                    .OrderByDescending(c => c.RespondedAt)
                    .Take(3)
                    .Select(c => new ProjectContributorDocument()
                    {
                        Id = c.Id,
                        ContributorFullName = c.Contributor.FullName,
                        ContributorProfilePictureUrl = c.Contributor.ProfilePictureObjectKey ?? "N/A",
                    })
                    .ToList(),
                OwnerId = p.OwnerId,
            })
            .ToListAsync();

        var userSO = await dbContext.Users
            .Include(u => u.UserProfile)
            .Include(u => u.UserSecuritySettings)
            .Include(u => u.Role)
            .ToListAsync();

        Console.WriteLine(userSO.Count());
        Console.WriteLine(userSO.Select(u => u.UserProfile).Count());
        Console.WriteLine(userSO.Select(u => u.UserSecuritySettings).Count());

        var usersF = userSO.Select(u => new UserDocument()
        {
            Id = u.Id,
            UserName = u.UserName,
            PreferredLanguage = u.PreferredLanguage,
            Role = u.Role.Type.ToString(),
            FullName = u.UserProfile.FullName,
            Email = u.Email,
            Status = u.Status,
            IsVerified = u.IsVerified,
            FailedLoginAttempts = u.FailedLoginAttempts,
            IsMfaEnabled = u.UserSecuritySettings.IsMfaEnabled,
            DefaultCommunicationMethod = u.UserSecuritySettings.DefaultCommunicationMethod,
            CreatedAt = u.CreatedAt,
            LastLogin = u.LastLogIn
        });


        foreach (var item in usersF)
        {
            Console.WriteLine(item.ToString());
        }

        //var users = await dbContext
        //    .Users
        //    .Select(u => new UserDocument()
        //    {
        //        Id = u.Id,
        //        UserName = u.UserName,
        //        PreferredLanguage = u.PreferredLanguage,
        //        Role = u.Role.Type.ToString(),
        //        FullName = u.UserProfile.FullName,
        //        Email = u.Email,
        //        Status = u.Status,
        //        IsVerified = u.IsVerified,
        //        FailedLoginAttempts = u.FailedLoginAttempts,
        //        IsMfaEnabled = u.UserSecuritySettings.IsMfaEnabled,
        //        DefaultCommunicationMethod = u.UserSecuritySettings.DefaultCommunicationMethod,
        //        CreatedAt = u.CreatedAt,
        //        LastLogin = u.LastLogIn
        //    })
        //    .ToListAsync();


        var profileSearchService = serviceProvider.GetRequiredService<IProfileSearchService>();
        var userSearchService = serviceProvider.GetRequiredService<IUserSearchService>();
        var projectSearchService = serviceProvider.GetRequiredService<IProjectSearchService>();
        var problemSearchService = serviceProvider.GetRequiredService<IProblemSearchService>();
        var jobSearchService = serviceProvider.GetRequiredService<IJobSearchService>();
        var postSearchService = serviceProvider.GetRequiredService<IPostSearchService>();
        var communitiesSearchService = serviceProvider.GetRequiredService<ICommunitySearchService>();


        var profileTask = profileSearchService.BulkProfilesAsync(profiles);
        var userTask = userSearchService.BulkUsersAsync(usersF.ToList());
        var projectTask = projectSearchService.BulkProjectsAsync(projects);
        var problemTask = problemSearchService.BulkProblemsAsync(problems);
        var jobTask = jobSearchService.BulkJobsAsync(jobs);
        var postTask = postSearchService.BulkPostsAsync(posts);
        var communitiesTask = communitiesSearchService.BulkCommunitiesAsync(communities);


        var result = await Task.WhenAll(profileTask, userTask, projectTask, problemTask, jobTask, postTask, communitiesTask);

        foreach (var res in result)
        {
            Console.WriteLine(res.ToString());
        }
    }
}