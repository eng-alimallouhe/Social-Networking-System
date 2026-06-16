using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Resumes.Bridges;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Infrastructure.Resumes.Repositories;

namespace SNS.Infrastructure.Resumes;

public static class ResumesInfrastructureDI
{
    public static IServiceCollection AddResumesInfrastructureDI(this IServiceCollection services)
    {
        // Soft Delete
        services.AddScoped<ISoftDeletableRepository<Resume>, ResumeRepository>();
        services.AddScoped<IRepository<ResumeCertificate>, ResumeCertificateRepository>();
        services.AddScoped<IRepository<ResumeEducation>, ResumeEducationRepository>();
        services.AddScoped<IRepository<ResumeExperience>, ResumeExperienceRepository>();
        services.AddScoped<IRepository<ResumeLanguage>, ResumeLanguageRepository>();
        services.AddScoped<IRepository<ResumeProject>, ResumeProjectRepository>();
        services.AddScoped<IRepository<ResumeSkill>, ResumeSkillRepository>();

        return services;
    }
}
