using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SNS.Infrastructure.ContentManagement;
using SNS.Infrastructure.Discussions;
using SNS.Infrastructure.Education.Repositories;
using SNS.Infrastructure.Identity.DependencyInjection;
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


        return services;
    }
}
