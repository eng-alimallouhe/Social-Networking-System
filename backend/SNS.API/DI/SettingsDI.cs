using SNS.Application.Settings;

namespace SNS.API.DI;

public static class SettingsDI
{
    public static IServiceCollection AddSettings(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(
            configuration.GetSection("AppSettings"));


        services.Configure<EmailSettings>(
            configuration.GetSection("EmailSettings"));


        services.Configure<JWTSettings>(
            configuration.GetSection("JWTSettings"));


        services.Configure<SmsSettings>(
            configuration.GetSection("SmsSettings"));

        return services;
    }
}
