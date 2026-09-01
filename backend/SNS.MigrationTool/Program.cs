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
        var permissionService = serviceProvider.GetRequiredService<SNS.Application.Identity.Shared.Abstractions.IPermissionService>();
        var logger = serviceProvider.GetService<ILogger<Program>>();
        await SNS.Infrastructure.Identity.Users.Seeding.PermissionSeeder.SeedPermissionsAndRolesAsync(dbContext, permissionService, logger);

        Console.WriteLine(
            BCrypt.Net.BCrypt.HashPassword(
                "alimallohi0947041713A"));

        var posts = await dbContext.Posts
            .Select(p => new PostDocument()
            {
                Id = p.Id,
                Title = p.Title,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Topics = p.PostTopics.Select(t => t.Topic.Name).ToList(),
                Tags = p.PostTags.Select(pt => pt.Tag.Name).ToList()
            })
            .ToListAsync();

        var profiles = await dbContext.Profiles
            .Select(p => new ProfileDocument()
            {
                Id = p.Id,
                FullName = p.FullName,
                Specialization = p.Specialization,
                Bio = p.Bio,
                Universities = p.AcademicRecords.Select(ar => ar.University.Name).ToList(),
                CreatedAt = p.CreatedAt,
                Skills = p.ProfileSkills.Select(s => s.Skill.Name).ToList()
            })
            .ToListAsync();

        var communities = await dbContext.Communities
            .Select(c => new CommunityDocument()
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                Type = c.Type,
                CreatedAt = c.CreatedAt
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
                CompanyName = j.Company.Name,
            })
            .ToListAsync();


        var problems = await dbContext.Problems
            .Select(p => new ProblemDocument()
            {
                Id = p.Id,
                Title = p.Title,
                Status = p.Status,
                Level = p.Level,
                ContentBlocks = p.ContentBlocks
                    .OrderBy(cb => cb.Order)
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
                ReadmeContent = p.ReadmeContent,
                Type = p.Type,
                Status = p.Status,
                PublishedAt = p.PublishedAt,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                Skills = p.Skills
                    .OrderByDescending(s => s.Id)
                    .Select(s => s.Skill.Name)
                    .ToList(),
                Tags = p.Tags.Select(t => t.Tag.Name).ToList()
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
            DefaultCommunicationMethod = u.UserSecuritySettings.DefaultCommunicationMethod,
            CreatedAt = u.CreatedAt
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