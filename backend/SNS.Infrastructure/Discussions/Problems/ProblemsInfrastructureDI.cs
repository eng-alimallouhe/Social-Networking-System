using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Discussions.Problems.Entities;
using SNS.Domain.Discussions.Problems.Relations;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Discussions.Problems.Repositories;
using SNS.Infrastructure.Repositories.QA;

namespace SNS.Infrastructure.Discussions.Problems;

public static class ProblemsInfrastructureDI
{
    public static IServiceCollection AddProblemsInfrastructureDI(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IRepository<ProblemTag>, ProblemTagRepository>();
        services.AddScoped<IRepository<ProblemContentBlock>, ProblemContentBlockRepository>();
        services.AddScoped<IRepository<ProblemTopic>, ProblemTopicRepository>();
        services.AddScoped<IRepository<ProblemVote>, ProblemVoteRepository>();
        services.AddScoped<IRepository<SavedProblem>, SavedProblemRepository>();
        services.AddScoped<ISoftDeletableRepository<Problem>, ProblemRepository>();
        services.AddScoped<ISoftDeletableRepository<ProblemView>, ProblemViewRepository>();


        return services;
    }
}
