using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Support.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Support.Repositories;

namespace SNS.Infrastructure.Support;

public static class SupportInfrastructureDI
{
    public static IServiceCollection AddSupportInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IRepository<SupportTicket>, SupportTicketRepository>();
        services.AddScoped<IRepository<TicketMessage>, TicketMessageRepository>();
        services.AddScoped<IRepository<TicketMessageAttachment>, TicketMessageAttachmentRepository>();

        return services;
    }
}
