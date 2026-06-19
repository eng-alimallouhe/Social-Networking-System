using MediatR;
using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Common;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.Queries.GetUserSecuritySettings;

public sealed record GetUserSecuritySettingsQuery(): IQuery<UserSecuritySettingsDto>;


public sealed class GetUserSecuritySettingsQueryHandler : IQueryHandler<GetUserSecuritySettingsQuery, UserSecuritySettingsDto>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IGeneratorService _generatorService;
    private readonly ICurrentUserService _currentUserService;

    public GetUserSecuritySettingsQueryHandler(
        IApplicationDbContext dbContext,
        IGeneratorService generatorService,
        ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _generatorService = generatorService;
        _currentUserService = currentUserService;
    }


    public async Task<Result<UserSecuritySettingsDto>> Handle(GetUserSecuritySettingsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null)
        {
            return Result<UserSecuritySettingsDto>.Failure(OperationStatusCode.AuthenticationRequired);
        }

        var settingsRaw = await _dbContext.UsersSecuritySettings
            .Where(uss => uss.UserId == userId)
            .Select(uss => new 
            {
                IsMfaEnabled = uss.IsMfaEnabled,
                IsAuthenticatorLinked = uss.IsAuthenticatorLinked,
                MfaProvider = uss.MfaProvider,
                RecoveryEmail = uss.RecoveryEmail,
                DefaultCommunicationMethod = uss.DefaultCommunicationMethod,
                ActiveRecoveryCodesCount = uss.RecoveryCodes.Count(),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (settingsRaw == null)
        {
            return Result<UserSecuritySettingsDto>.Failure(UserStatusCodes.NotFound);
        }

        return Result<UserSecuritySettingsDto>.Success(new UserSecuritySettingsDto(
                IsMfaEnabled: settingsRaw.IsMfaEnabled,
                IsAuthenticatorLinked: settingsRaw.IsAuthenticatorLinked,
                MfaProvider: settingsRaw.MfaProvider,
                RecoveryEmail: settingsRaw.RecoveryEmail != null
                    ? _generatorService.GenerateEmailMask(settingsRaw.RecoveryEmail)
                    : null,
                DefaultCommunicationMethod: settingsRaw.DefaultCommunicationMethod,
                ActiveRecoveryCodesCount: settingsRaw.ActiveRecoveryCodesCount
            ), ResourceStatusCode.Found);
    }
}
