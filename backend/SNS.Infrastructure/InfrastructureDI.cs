using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SNS.Infrastructure.ContentManagement;
using SNS.Infrastructure.Discussions;
using SNS.Infrastructure.Education.Repositories;
using SNS.Infrastructure.Identity;
using SNS.Infrastructure.Jobs;
using SNS.Infrastructure.Preferences;
using SNS.Infrastructure.Profiles;
using SNS.Infrastructure.Projects;
using SNS.Infrastructure.Resumes;
using SNS.Infrastructure.Search;
using SNS.Infrastructure.Shared;
using SNS.Infrastructure.Shared.Services.Cashing;

namespace SNS.Infrastructure;

public static class InfrastructureDI
{
    public static IServiceCollection AddInfrastructureDI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddContentManagementInfrastructureDI();
        services.AddDiscussionsInfrastructureDI();
        services.AddEducationInfrastructureDI();
        services.AddIdentityInfrastructureDI();
        services.AddJobsInfrastructureDI();
        services.AddPreferencesInfrastructureDI();
        services.AddProfileContextInfrastructureDI();
        services.AddProjectsInfrastructureDI();
        services.AddSearchInfrastructureDI();
        services.AddResumesInfrastructureDI();
        services.AddSharedInfrastructureDI(configuration);
        services.AddCachingServices(configuration);

        var serviceAccountPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Shared",
            "Resources", 
            "Firebase", 
            "service-account.json");

        if (string.IsNullOrWhiteSpace(serviceAccountPath))
        {
            throw new InvalidOperationException(
                "Firebase:ServiceAccountPath is missing.");
        }

        if (!File.Exists(serviceAccountPath))
        {
            throw new FileNotFoundException(
                "Firebase service account file was not found.",
                serviceAccountPath);
        }

        Environment.SetEnvironmentVariable(
            "GOOGLE_APPLICATION_CREDENTIALS",
            serviceAccountPath);

        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.GetApplicationDefault()
            });
        }

        services.AddHangfire(conf => conf
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            }));

        services.AddHangfireServer();

        return services;
    }
}
