using Microsoft.Extensions.DependencyInjection;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Resumes.Bridges;
using SNS.Domain.Resumes.Entities;

namespace SNS.Infrastructure.Repositories.Resumes;

public static class ResumesDI
{
    public static IServiceCollection AddResumesRepositories(
        this IServiceCollection services)
    {
        // Soft Delete
        services.AddScoped<
            ISoftDeletableRepository<Resume>, 
            ResumeRepository>();

        // Hard Delete
        services.AddScoped<
            IRepository<ResumeCertificate>, 
            ResumeCertificateRepository>();

        services.AddScoped<
            IRepository<ResumeEducation>, 
            ResumeEducationRepository>();

        services.AddScoped<
            IRepository<ResumeExperience>, 
            ResumeExperienceRepository>();

        services.AddScoped<
            IRepository<ResumeLanguage>, 
            ResumeLanguageRepository>();

        services.AddScoped<
            IRepository<ResumeProject>, 
            ResumeProjectRepository>();

        services.AddScoped<
            IRepository<ResumeSkill>, 
            ResumeSkillRepository>();


        return services;
    }
}