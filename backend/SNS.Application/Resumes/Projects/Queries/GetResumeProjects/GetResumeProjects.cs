using Microsoft.EntityFrameworkCore;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Resumes.Projects.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Resumes.Projects.Queries.GetResumeProjects;

/// <summary>
/// Represents a query to retrieve all projects linked to a specific resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the target resume.</param>
public sealed record GetResumeProjectsQuery(Guid ResumeId) : IQuery<List<ResumeProjectDto>>;

/// <summary>
/// Handles the execution of <see cref="GetResumeProjectsQuery"/> to fetch linked project summaries.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Joins the resume project bridge with active project entities.
/// 2. Resolves presigned temporary URLs for project cover images.
/// 3. Projects records to <see cref="ResumeProjectDto"/> list.
/// </remarks>
internal sealed class GetResumeProjectsQueryHandler : IQueryHandler<GetResumeProjectsQuery, List<ResumeProjectDto>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;

    public GetResumeProjectsQueryHandler(
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService)
    {
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
    }

    public async Task<Result<List<ResumeProjectDto>>> Handle(GetResumeProjectsQuery request, CancellationToken cancellationToken)
    {
        var rawProjects = await (
            from rp in _dbContext.ResumeProjects
            join p in _dbContext.Projects on rp.ProjectId equals p.Id
            where rp.ResumeId == request.ResumeId && p.IsActive
            select new
            {
                rp.ResumeId,
                rp.ProjectId,
                p.Title,
                p.ShortDescription,
                p.MainImageUrl,
                p.Type,
                p.Status
            }
        ).ToListAsync(cancellationToken);

        var imageKeys = rawProjects
            .Select(p => p.MainImageUrl)
            .Where(k => !string.IsNullOrWhiteSpace(k))
            .Distinct()
            .ToList();

        Dictionary<string, string> imageMap = new();
        if (imageKeys.Any())
        {
            var imgTasks = imageKeys.Select(async k => new
            {
                Key = k!,
                Url = await _fileStorageService.GetTemporaryUrlAsync(k!, TimeSpan.FromHours(1))
            });
            var resolved = await Task.WhenAll(imgTasks);
            imageMap = resolved.ToDictionary(x => x.Key, x => x.Url);
        }

        var dtos = rawProjects.Select(p => new ResumeProjectDto(
            p.ResumeId,
            p.ProjectId,
            p.Title,
            p.ShortDescription,
            !string.IsNullOrWhiteSpace(p.MainImageUrl) && imageMap.TryGetValue(p.MainImageUrl, out var url) ? url : p.MainImageUrl,
            p.Type,
            p.Status
        )).ToList();

        return Result<List<ResumeProjectDto>>.Success(dtos, ResourceStatusCode.Found);
    }
}
