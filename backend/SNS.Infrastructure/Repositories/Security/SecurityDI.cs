using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Security.Entities;

namespace SNS.Infrastructure.Repositories.Security;


public static class SecurityDI
{
    public static IServiceCollection AddSecurityRepositories(
        this IServiceCollection services)
    {
        // Soft Delete
        services.AddScoped<ISoftDeletableRepository<User>, UserRepository>();
        services.AddScoped<ISoftDeletableRepository<Role>, RoleRepository>();
        services.AddScoped<ISoftDeletableRepository<SupportTicket>, SupportTicketRepository>();

        // Hard Delete
        services.AddScoped<IRepository<RefreshToken>, RefreshTokenRepository>();
        services.AddScoped<IRepository<VerificationCode>, VerificationCodeRepository>();
        services.AddScoped<IRepository<UserSession>, UserSessionRepository>();
        services.AddScoped<IRepository<UserArchive>, UserArchiveRepository>();
        services.AddScoped<IRepository<IdentityArchive>, IdentityArchiveRepository>();
        services.AddScoped<IRepository<PasswordArchive>, PasswordArchiveRepository>();
        services.AddScoped<IRepository<PendingUpdate>, PendingUpdateRepository>();
        services.AddScoped<IRepository<Notification>, NotificationRepository>();
        services.AddScoped<IRepository<SupportResponse>, SupportResponseRepository>();
        services.AddScoped<IRepository<ManualRecoveryRequest>, ManualRecoveryRequestRepository>();

        return services;
    }
}