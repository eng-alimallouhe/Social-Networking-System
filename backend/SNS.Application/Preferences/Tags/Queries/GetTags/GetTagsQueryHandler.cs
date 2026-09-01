using Microsoft.EntityFrameworkCore;
using SNS.Application.Preferences.Tags.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Preferences.Tags.Queries.GetTags;

/// <summary>
/// Handles the execution of <see cref="GetTagsQuery"/> to retrieve at most 10 matching tags for autocomplete.
/// </summary>
/// <remarks>
/// Data retrieval and query logic:
/// 1. Queries the read-only tags dataset using <see cref="IApplicationDbContext"/>.
/// 2. Applies <c>AsNoTracking()</c> for high-performance read-only access.
/// 3. Filters tags by name if a search keyword is provided.
/// 4. Applies deterministic ordering by tag name.
/// 5. Limits results to at most 10 items.
/// 6. Projects only required fields (<c>Id</c> and <c>Name</c>) into <see cref="TagDto"/>.
/// </remarks>
internal sealed class GetTagsQueryHandler : IQueryHandler<GetTagsQuery, List<TagDto>>
{
    private readonly IApplicationDbContext _dbContext;

    public GetTagsQueryHandler(IApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<TagDto>>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var query = _dbContext.Tags.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(t => t.Name.Contains(search));
        }

        var tags = await query
            .OrderBy(t => t.Name)
            .Take(10)
            .Select(t => new TagDto(t.Id, t.Name))
            .ToListAsync(cancellationToken);

        return Result<List<TagDto>>.Success(tags, OperationStatusCode.Success);
    }
}
