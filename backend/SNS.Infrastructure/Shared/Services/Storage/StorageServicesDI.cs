using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using SNS.Application.Shared.Settings;
using SNS.Application.Shared.Abstractions.Storage;

namespace SNS.Infrastructure.Shared.Services.Storage;

public static class StorageServicesDI
{
    public static IServiceCollection AddStorageServices(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        var minioSettings = new MinioStorageSettings();
        configuration.GetSection("MinioStorage").Bind(minioSettings);

        services.Configure<MinioStorageSettings>(configuration.GetSection("MinioStorage"));

        services.AddMinio(configureClient => configureClient
            .WithEndpoint(minioSettings.Endpoint)
            .WithCredentials(minioSettings.AccessKey, minioSettings.SecretKey)
            .WithSSL(minioSettings.UseSSL)
            .Build());

        services.AddScoped<IFileStorageService, MinioFileStorageService>();
    
        return services;
    }
}
