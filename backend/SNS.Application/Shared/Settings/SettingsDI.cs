using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SNS.Application.Shared.Settings;

public static class SettingsDI
{
    public static IServiceCollection AddSettingsDI(
        this IServiceCollection services,
        IConfiguration configuration)
    {

        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
        services.Configure<MinioStorageSettings>(configuration.GetSection("MinioSettings"));
        services.Configure<ProfileSettings>(configuration.GetSection("ProfileSettings"));
        services.Configure<ReputationSettings>(configuration.GetSection("ReputationSettings"));
        services.Configure<SmsSettings>(configuration.GetSection("SmsSettings"));

        return services;
    }
}
