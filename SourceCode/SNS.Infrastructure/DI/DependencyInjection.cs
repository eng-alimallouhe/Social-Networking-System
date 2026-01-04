namespace SNS.Infrastructure.DI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SNS.Infrastructure.Persistence;
using SNS.Infrastructure.Repositories;
using SNS.Infrastructure.Services;
using SNS.Infrastructure.Services.Cashing;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddPersistence(configuration)
            .AddCashingServices(configuration)
            .AddRepositories()
            .AddInfrastructureService();


        return services;
    }
}