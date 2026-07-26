using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Shared.Abstractions.AI;

namespace SNS.Infrastructure.Shared.Services.AI;

public static class AIDI
{
    public static IServiceCollection AddAIDI(
        this IServiceCollection services)
    {
        services.AddScoped<ITopicClassifier, TopicClassifier>();

        return services;
    }
}
