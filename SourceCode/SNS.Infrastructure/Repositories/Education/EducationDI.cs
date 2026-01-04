using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Education.Entities;

namespace SNS.Infrastructure.Repositories.Education;

public static class EducationDI
{
    public static IServiceCollection AddEducationRepositories(
        this IServiceCollection services)
    {
        // Soft Delete
        services.AddScoped<
            ISoftDeletableRepository<University>, 
            UniversityRepository>();
        
        services.AddScoped<
            ISoftDeletableRepository<Faculty>, 
            FacultyRepository>();

        // Hard Delete
        services.AddScoped<
            IRepository<FacultyRequest>, 
            FacultyRequestRepository>();
        
        services.AddScoped<
            IRepository<UniversityRequest>,
            UniversityRequestRepository>();
       
        services.AddScoped<
            IRepository<ProfileFacultyRequest>, 
            ProfileFacultyRequestRepository>();
        
        services.AddScoped<
            IRepository<ProfileUniversityRequest>,
            ProfileUniversityRequestRepository>();

        return services;
    }
}