using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.QA.Bridges;
using SNS.Domain.QA.Entities;

namespace SNS.Infrastructure.Repositories.QA;

public static class QADi
{
    public static IServiceCollection AddQARepositories(
        this IServiceCollection services)
    {
        // Soft Delete
        services.AddScoped<
            ISoftDeletableRepository<Discussion>, 
            DiscussionRepository>();

        services.AddScoped<
            ISoftDeletableRepository<Problem>, 
            ProblemRepository>();

        services.AddScoped<
            ISoftDeletableRepository<Solution>, 
            SolutionRepository>();


        // Hard Delete
        services.AddScoped<
            IRepository<ProblemContentBlock>, 
            ProblemContentBlockRepository>();

        services.AddScoped<
            IRepository<SolutionContentBlock>, 
            SolutionContentBlockRepository>();

        services.AddScoped<
            IRepository<ProblemTag>, 
            ProblemTagRepository>();

        services.AddScoped<
            IRepository<ProblemTopic>, 
            ProblemTopicRepository>();

        services.AddScoped<
            ISoftDeletableRepository<ProblemView>, 
            ProblemViewRepository>();

        services.AddScoped<
            IRepository<ProblemVote>, 
            ProblemVoteRepository>();

        services.AddScoped<
            IRepository<SolutionVote>, 
            SolutionVoteRepository>();


        return services;
    }
}