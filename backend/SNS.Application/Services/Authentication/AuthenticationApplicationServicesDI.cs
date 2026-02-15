using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Abstractions.Authentication;

namespace SNS.Application.Services.Authentication;

public static class AuthenticationApplicationServicesDI
{
    public static IServiceCollection AddAuthenticationApplicationServices(
        this IServiceCollection services)
    {
        services.AddScoped<
            IAccountService, AccountService>();

        services.AddScoped<
            IRegisterService, RegisterService>();

        services.AddScoped<
            ICodeService, CodeService>();

        return services;
    }
}
