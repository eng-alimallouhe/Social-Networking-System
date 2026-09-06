using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SNS.Application.Shared.Abstractions.AI;
using SNS.Application.Shared.Contracts.AI;

namespace SNS.Infrastructure.Shared.Services.AI;

public static class AIDI
{
    public static IServiceCollection AddAIDI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<QwenOptions>(
            configuration.GetSection(QwenOptions.SectionName));

        services.AddHttpClient<IQwenClient, QwenClient>((serviceProvider, client) =>
        {
            var options = serviceProvider
                .GetRequiredService<IOptions<QwenOptions>>()
                .Value;

            client.BaseAddress = new Uri(options.BaseUrl);
        });

        services.AddScoped<ITopicClassifier, TopicClassifier>();

        return services;
    }
}