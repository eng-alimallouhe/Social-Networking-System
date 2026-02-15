using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Abstractions.Messaging;

namespace SNS.Infrastructure.Services.Messaging;

public static class MessaginServiceDI
{
    public static IServiceCollection AddMessaginService(
        this IServiceCollection services)
    {
        services.AddScoped<
            IEmailSenderService, EmailSenderService>();

        services.AddScoped<
            ISmsSenderService, SmsSenderService>();

        services.AddScoped<
            ITemplateReaderService, TemplateReaderService>();

        services.AddScoped<
            ISmsStructureReader, SmsStructureReader>();

        return services;
    }
}
