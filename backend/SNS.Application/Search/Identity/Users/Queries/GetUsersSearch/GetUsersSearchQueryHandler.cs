using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.DTOs.Users;
using SNS.Application.Search.Identity.Users.Abstractions;
using SNS.Application.Search.Shared.Contracts;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Search.Identity.Users.Queries.GetUsersSearch;

/// <summary>
/// Handles the execution of <see cref="GetUsersSearchQuery"/> to search user accounts and return authoritative user summaries.
/// </summary>
public class GetUsersSearchQueryHandler
: IQueryHandler<GetUsersSearchQuery, SearchResult<UserSummaryDto>>
{
    private readonly IUserSearchService _userSearchService;
    private readonly IApplicationDbContext _dbContext;

    public GetUsersSearchQueryHandler(
        IUserSearchService userSearchService,
        IApplicationDbContext dbContext)
    {
        _userSearchService = userSearchService;
        _dbContext = dbContext;
    }

    public async Task<Result<SearchResult<UserSummaryDto>>> Handle(
        GetUsersSearchQuery request,
        CancellationToken cancellationToken)
    {
        var searchResult = await _userSearchService.SearchUsersAsync(request, cancellationToken);
        if (!searchResult.Hits.Any())
        {
            return Result<SearchResult<UserSummaryDto>>.Success(new SearchResult<UserSummaryDto>
            {
                Hits = new List<SearchHit<UserSummaryDto>>(),
                Total = searchResult.Total
            }, OperationStatusCode.Success);
        }

        var userIds = searchResult.Hits.Select(h => h.Document.Id).ToList();

        var users = await _dbContext.Users
            .Where(u => userIds.Contains(u.Id))
            .Select(u => new UserSummaryDto(
                u.Id,
                u.UserName,
                u.UserProfile != null ? u.UserProfile.FullName : null,
                u.Email,
                u.Role.Type.ToString(),
                u.Status,
                u.PreferredLanguage,
                u.UserSecuritySettings.DefaultCommunicationMethod,
                u.CreatedAt,
                u.UserProfile != null ? u.UserProfile.ProfilePictureObjectKey : null
            ))
            .ToListAsync(cancellationToken);

        var orderedHits = searchResult.Hits
            .Select(hit =>
            {
                var userDto = users.FirstOrDefault(u => u.Id == hit.Document.Id);
                return userDto != null ? new SearchHit<UserSummaryDto>(userDto, hit.Score) : null;
            })
            .Where(h => h != null)
            .Select(h => h!)
            .ToList();

        return Result<SearchResult<UserSummaryDto>>.Success(new SearchResult<UserSummaryDto>
        {
            Hits = orderedHits,
            Total = searchResult.Total
        }, OperationStatusCode.Success);
    }
}
