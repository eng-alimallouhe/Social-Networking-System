using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Identity.Users.Repositories;

namespace SNS.Infrastructure.Identity.Users;

public static class UsersInfrastructureDI
{
    public static IServiceCollection AddUsersInfrastructureDI(this IServiceCollection services)
    {
        services.AddScoped<IRepository<User>, UserRepository>();
        services.AddScoped<ISoftDeletableRepository<Role>, RoleRepository>();

        return services;
    }
}
