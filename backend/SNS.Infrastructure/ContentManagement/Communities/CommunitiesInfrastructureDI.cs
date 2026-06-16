using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.ContentManagement.Communities.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;

namespace SNS.Infrastructure.ContentManagement.Communities.Repositories;


public static class CommunitiesInfrastructureDI
{
    public static IServiceCollection AddCommunitiesInfrastructureDI(
        this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<ISoftDeletableRepository<Community>, CommunityRepository>();
        services.AddScoped<IRepository<CommunityAuditLog>, CommunityAuditLogRepository>();
        services.AddScoped<IRepository<CommunityInvitation>, CommunityInvitationRepository>();
        services.AddScoped<IRepository<CommunityJoinRequest>, CommunityJoinRequestRepository>();
        services.AddScoped<IRepository<CommunityMembership>, CommunityMembershipRepository>();
        services.AddScoped<IRepository<CommunityRule>, CommunityRuleRepository>();
        services.AddScoped<IRepository<CommunitySettings>, CommunitySettingsRepository>();


        return services;
    }
}
