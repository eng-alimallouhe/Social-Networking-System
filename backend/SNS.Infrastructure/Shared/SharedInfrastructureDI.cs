using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Shared.BackgroundServices;
using SNS.Infrastructure.Shared.Hangfire;
using SNS.Infrastructure.Shared.Repositories;
using SNS.Infrastructure.Shared.Services;
using SNS.Infrastructure.Shared.Services.Cashing;

namespace SNS.Infrastructure.Shared;

public static class SharedInfrastructureDI
{
    public static IServiceCollection AddSharedInfrastructureDI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddQuartzHostedService(options =>
        {
            options.WaitForJobsToComplete = true;
        });

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

        services
            .AddScoped<IUnitOfWork, UnitOfWork>()
            .AddPersistenceDI(configuration)
            .AddSharedServiceInfrastructureDI(configuration)
            .AddBackgroundJobsServices()
            .AddCachingServices(configuration)
            .AddHangfireDI(configuration);

        return services;
    }
}
