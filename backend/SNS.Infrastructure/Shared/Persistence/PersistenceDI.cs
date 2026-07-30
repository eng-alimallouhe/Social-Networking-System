using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Shared.Abstractions.Data;

namespace SNS.Infrastructure.Persistence;

public static class PersistenceDI
{
    public static IServiceCollection AddPersistenceDI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<SNSDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"));
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<SNSDbContext>());

        return services;
    }
}
