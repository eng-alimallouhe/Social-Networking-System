using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Abstractions.Messaging;

namespace SNS.Infrastructure.Shared.Services.Messaging;

public static class MessaginServiceDI
{
    public static IServiceCollection AddMessagingService(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<
            IEmailSenderService, EmailSenderService>();


        services.AddScoped<
            IEmailTemplateProvider, EmailTemplateProvider>();

        return services;
    }
}
