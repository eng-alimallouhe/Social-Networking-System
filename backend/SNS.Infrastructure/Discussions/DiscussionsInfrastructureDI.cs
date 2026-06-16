using Microsoft.Extensions.DependencyInjection;
using SNS.Infrastructure.Discussions.Problems;
using SNS.Infrastructure.Discussions.Solutions;

namespace SNS.Infrastructure.Discussions;

public static class DiscussionsInfrastructureDI
{
    public static IServiceCollection AddDiscussionsInfrastructureDI(this IServiceCollection services)
    {
        services.AddProblemsInfrastructureDI()
                .AddSolutionsInfrastructureDI();

        return services;
    }
}
