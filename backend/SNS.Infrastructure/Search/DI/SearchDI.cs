using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Search.ContentManagement.Communitites.Abstractions;
using SNS.Application.Search.ContentManagement.Posts.Abstractions;
using SNS.Application.Search.Identity.Users.Abstractions;
using SNS.Application.Search.Jobs.Abstractions;
using SNS.Application.Search.Profiles.Profiles.Abstractions;
using SNS.Application.Search.Projects.Abstractions;
using SNS.Application.Search.Discussions.Problems.Abstractions;
using SNS.Infrastructure.Search.Abstractions;
using SNS.Infrastructure.Search.Services;

namespace SNS.Infrastructure.Search.DI;

public static class SearchDI
{
    public static IServiceCollection AddSearchServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton(sp =>
        {
            var elasticSearchUrl =
                configuration["ElasticSearch:Url"]
                ?? throw new InvalidOperationException(
                    "ElasticSearch:Url is not configured.");

            var settings = new ElasticsearchClientSettings(
                new Uri(elasticSearchUrl)
            );

            return new ElasticsearchClient(settings);
        });

        services.AddScoped(
            typeof(IElasticDocumentService<>),
            typeof(ElasticDocumentService<>));

        services.AddScoped<IUserSearchService, UserSearchService>();
        services.AddScoped<IProjectSearchService, ProjectSearchService>();
        services.AddScoped<IProfileSearchService, ProfileSearchService>();
        services.AddScoped<IJobSearchService, JobSearchService>();
        services.AddScoped<IProblemSearchService, ProblemSearchService>();
        services.AddScoped<ICommunitySearchService, CommunitySearchService>();
        services.AddScoped<IPostSearchService, PostSearchService>();

        return services;
    }
}