using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Abstractions.Authentication;

namespace SNS.Infrastructure.Services.Authentication;

public static class AuthenticationInfrastructureServicesDI
{
    public static IServiceCollection AddAuthenticationInfrastructureServices(
        this IServiceCollection services)
    {
        services.AddScoped<
            ITokenService, TokenService>();

        services.AddScoped<
            ITokenReaderService, TokenReaderService>();

        return services;
    }
}
