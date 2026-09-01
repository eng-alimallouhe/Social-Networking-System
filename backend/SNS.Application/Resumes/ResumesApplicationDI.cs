using Microsoft.Extensions.DependencyInjection;
using SNS.Application.Resumes.Resumes.Services;

namespace SNS.Application.Resumes;

public static class ResumesApplicationDI
{
    public static IServiceCollection AddResumesApplicationDI(this IServiceCollection services)
    {
        services.AddScoped<IResumeUrlResolver, ResumeUrlResolver>();

        return services;
    }
}
