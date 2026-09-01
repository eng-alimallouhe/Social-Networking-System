using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Domain.Projects.ValueObjects;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Projects.Queries.GetProjectSourceCode;

internal sealed class GetProjectSourceCodeQueryHandler : IQueryHandler<GetProjectSourceCodeQuery, List<FileNode>>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _storageService;

    public GetProjectSourceCodeQueryHandler(IApplicationDbContext dbContext, IFileStorageService storageService)
    {
        _dbContext = dbContext;
        _storageService = storageService;
    }

    public async Task<Result<List<FileNode>>> Handle(GetProjectSourceCodeQuery request, CancellationToken cancellationToken)
    {
        var project = await _dbContext.Projects
            .Where(p => p.Id == request.ProjectId && p.IsActive)
            .Select(p => new { p.SourceCodeTree })
            .FirstOrDefaultAsync(cancellationToken);

        if (project == null)
        {
            return Result<List<FileNode>>.Failure(ResourceStatusCode.NotFound);
        }

        if (string.IsNullOrEmpty(project.SourceCodeTree))
        {
            return Result<List<FileNode>>.Success(new List<FileNode>(), OperationStatusCode.Success);
        }

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var tree = JsonSerializer.Deserialize<List<FileNode>>(project.SourceCodeTree, jsonOptions);

        if (tree == null)
        {
            return Result<List<FileNode>>.Success(new List<FileNode>(), OperationStatusCode.Success);
        }

        await ProcessFileNodesAsync(tree);

        return Result<List<FileNode>>.Success(tree, OperationStatusCode.Success);
    }

    private async Task ProcessFileNodesAsync(IEnumerable<FileNode> nodes)
    {
        foreach (var node in nodes)
        {
            if (node.Type != "folder" && !string.IsNullOrEmpty(node.Url))
            {
                node.Url = await _storageService.GetTemporaryUrlAsync(node.Url, TimeSpan.FromHours(1));
            }

            if (node.Children != null && node.Children.Any())
            {
                await ProcessFileNodesAsync(node.Children);
            }
        }
    }
}
