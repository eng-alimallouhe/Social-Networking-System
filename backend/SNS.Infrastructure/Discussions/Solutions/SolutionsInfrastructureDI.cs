using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Discussions.Solutions.Entities;
using SNS.Domain.Discussions.Solutions.Relations;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Repositories.QA;

namespace SNS.Infrastructure.Discussions.Solutions;

public static class SolutionsInfrastructureDI
{
    public static IServiceCollection AddSolutionsInfrastructureDI(this IServiceCollection services)
    {
        services.AddScoped<ISoftDeletableRepository<Solution>, SolutionRepository>();
        services.AddScoped<ISoftDeletableRepository<Discussion>, DiscussionRepository>();
        services.AddScoped<IRepository<SolutionContentBlock>, SolutionContentBlockRepository>();
        services.AddScoped<IRepository<SolutionVote>, SolutionVoteRepository>();
        services.AddScoped<IRepository<SavedSolution>, SavedSolutionRepository>();

        return services;
    }
}
