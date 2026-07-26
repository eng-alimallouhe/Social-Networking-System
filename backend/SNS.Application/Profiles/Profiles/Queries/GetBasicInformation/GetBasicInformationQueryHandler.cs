using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Application.Shared.Abstractions.Storage;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using Microsoft.EntityFrameworkCore;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Profiles.Profiles.Queries.GetBasicInformation;

public sealed class GetBasicInformationQueryHandler : IQueryHandler<GetBasicInformationQuery, BasicInformationDto>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IApplicationDbContext _dbContext;
    private readonly IFileStorageService _fileStorageService;
    public GetBasicInformationQueryHandler(
        ICurrentUserService currentUserService,
        IApplicationDbContext dbContext,
        IFileStorageService fileStorageService)
    {
        _currentUserService = currentUserService;
        _dbContext = dbContext;
        _fileStorageService = fileStorageService;
    }
    public async Task<Result<BasicInformationDto>> Handle(GetBasicInformationQuery request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;

        if (profileId == null)
        {
            return Result<BasicInformationDto>.Failure(SecurityStatusCodes.AuthenticationRequired); ;
        }

        var profile = await _dbContext
            .Profiles
            .Where(p => p.Id == profileId)
            .Select(p => new BasicInformationDto(
                FullName: p.FullName,
                Bio: p.Bio,
                ProfilePictureUrl: null,
                Specialization: p.Specialization,
                Reputation: p.Reputation
            ))
            .FirstOrDefaultAsync(cancellationToken);
        
        if (profile == null)
        {
            return Result<BasicInformationDto>.Failure(ResourceStatusCode.NotFound);
        }
        
        return Result<BasicInformationDto>.Success(profile, ResourceStatusCode.Found);
    }
}
