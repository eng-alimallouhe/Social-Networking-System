using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Educations.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;

namespace SNS.Infrastructure.Education.Repositories;

public static class EducationInfrastructureDI
{
    public static IServiceCollection AddEducationInfrastructureDI(
        this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<ISoftDeletableRepository<University>, UniversityRepository>();
        services.AddScoped<IRepository<AcademicRecord>, AcademicRecordRepository>();


        return services;
    }
}
