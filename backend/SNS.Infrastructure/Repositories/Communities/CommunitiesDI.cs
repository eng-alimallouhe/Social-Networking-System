using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Communities.Entities;

namespace SNS.Infrastructure.Repositories.Communities;

public static class CommunitiesDI
{
    public static IServiceCollection AddCommunitiesRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<
            ISoftDeletableRepository<Community>, 
            CommunityRepository>();

        services.AddScoped<
            IRepository<CommunityAuditLog>, 
            CommunityAuditLogRepository>();
        
        services.AddScoped<
            IRepository<CommunityCreationRequest>,
            CommunityCreationRequestRepository>();
        
        services.AddScoped<
            IRepository<CommunityInvitation>, 
            CommunityInvitationRepository>();
        
        services.AddScoped<
            IRepository<CommunityJoinRequest>, 
            CommunityJoinRequestRepository>();
        
        services.AddScoped<
            IRepository<CommunityMembership>, 
            CommunityMembershipRepository>();
        
        services.AddScoped<
            IRepository<CommunityRule>, 
            CommunityRuleRepository>();
        
        services.AddScoped<
            IRepository<CommunitySettings>, 
            CommunitySettingsRepository>();

        return services;
    }
}