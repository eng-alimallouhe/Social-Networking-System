using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using Microsoft.EntityFrameworkCore;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Profiles.Profiles.Queries.GetProfilePictureUrl;

public sealed class GetProfilePictureUrlQueryHandler : IQueryHandler<GetProfilePictureUrlQuery, string>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;
    public GetProfilePictureUrlQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
    }
    public async Task<Result<string>> Handle(GetProfilePictureUrlQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        
        if (!profileId.HasValue)
        {
            return Result<string>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }
        
        var profilePictureObjectKey = await _dbContext
            .Profiles
            .Where(p => p.Id == profileId.Value)
            .Select(p => p.ProfilePictureObjectKey)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(profilePictureObjectKey))
        {
            return Result<string>.Failure(ResourceStatusCode.NotFound);
        }

        var temporaryUrl = await _fileStorageService.GetTemporaryUrlAsync(profilePictureObjectKey, TimeSpan.FromMinutes(15));
        
        return Result<string>.Success(temporaryUrl, OperationStatusCode.Success);
    }
}