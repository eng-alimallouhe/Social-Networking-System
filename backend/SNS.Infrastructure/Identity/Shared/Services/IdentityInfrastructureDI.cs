using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Identity.SecuritySessions.Shared.Abstractions;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Settings;

namespace SNS.Infrastructure.Identity.Shared.Services;

public static class IdentityInfrastructureDI
{
    public static IServiceCollection AddIdentityInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JWTSettings>(
            configuration.GetSection("JWTSettings"));

        services.AddScoped<
            ICurrentUserService, CurrentUserService>();

        services.AddScoped<
            IRequestInfoService, RequestInfoService>();

        services.AddScoped<
            ITokenReaderService, TokenReaderService>();

        services.AddScoped<
            ITokenService, TokenService>();

        return services; 
    }
}
