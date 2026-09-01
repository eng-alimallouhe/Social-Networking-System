using Microsoft.EntityFrameworkCore;
using SNS.Application.Projects.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.DTOs;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using System.Linq;

namespace SNS.Application.Projects.Queries.GetProjectMedia;

internal sealed class GetProjectMediaQueryHandler : IQueryHandler<GetProjectMediaQuery, Paged<ProjectMediaDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetProjectMediaQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Paged<ProjectMediaDto>>> Handle(GetProjectMediaQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.ProjectMedia
            .Where(m => m.ProjectId == request.ProjectId)
            .OrderBy(m => m.CreatedAt)
            .ThenBy(m => m.Id)
            .Select(m => new ProjectMediaDto(
                m.Id,
                m.MediaUrl,
                m.Type.ToString(),
                0, // No order property in entity
                m.CreatedAt
            ));

        var count = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var paged = new Paged<ProjectMediaDto>(items, count, request.PageSize, request.Page);

        return Result<Paged<ProjectMediaDto>>.Success(paged, OperationStatusCode.Success);
    }
}
