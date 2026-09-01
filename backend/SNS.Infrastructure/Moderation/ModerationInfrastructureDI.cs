using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Moderation.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Moderation.Repositories;

namespace SNS.Infrastructure.Moderation;

public static class ModerationInfrastructureDI
{
    public static IServiceCollection AddModerationInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IRepository<ContentReport>, ContentReportRepository>();
        services.AddScoped<IRepository<ReportTicket>, ReportTicketRepository>();

        return services;
    }
}
