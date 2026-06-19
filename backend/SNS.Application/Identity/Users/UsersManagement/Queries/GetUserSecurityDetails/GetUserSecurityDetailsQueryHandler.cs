using Microsoft.EntityFrameworkCore;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Abstractions.Messaging;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Users.UsersManagement.Queries.GetUserSecurityDetails;

public sealed class GetUserSecurityDetailsQueryHandler : IQueryHandler<GetUserSecurityDetailsQuery, UserSecurityDetailsResult>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;

    public GetUserSecurityDetailsQueryHandler(IApplicationDbContext dbContext, ICurrentUserService currentUserService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
    }

    public async Task<Result<UserSecurityDetailsResult>> Handle(GetUserSecurityDetailsQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null || userId == Guid.Empty)
        {
            return Result<UserSecurityDetailsResult>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        // 1️⃣ جلب كافة تفاصيل الأمان المتقدمة بضربة واحدة باستغلال علاقات الكيان الحالية (Passkeys, Devices, Settings, Codes)
        var securityData = await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId)
            .Select(u => new
            {
                u.LastPasswordChange,
                IsMfaEnabled = u.UserSecuritySettings.IsMfaEnabled,
                MfaProvider = u.UserSecuritySettings.MfaProvider.ToString(),
                IsAuthenticatorAppLinked = u.UserSecuritySettings.IsAuthenticatorLinked,
                RecoveryEmail = u.UserSecuritySettings.RecoveryEmail,
                PasskeysCount = u.Passkeys.Count(),
                TotalDevicesCount = u.Devices.Count(),
                UsedCodes = _dbContext.RecoveryCodes.Count(rc => rc.UserSecuritySettingsId == u.UserSecuritySettings.Id && rc.IsUsed),
                UnusedCodes = _dbContext.RecoveryCodes.Count(rc => rc.UserSecuritySettingsId == u.UserSecuritySettings.Id && !rc.IsUsed)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (securityData == null)
        {
            return Result<UserSecurityDetailsResult>.Failure(ResourceStatusCode.NotFound);
        }

        var result = new UserSecurityDetailsResult(
            IsMfaEnabled: securityData.IsMfaEnabled,
            MfaProvider: securityData.MfaProvider,
            IsAuthenticatorAppLinked: securityData.IsAuthenticatorAppLinked,
            PasskeysCount: securityData.PasskeysCount,
            LastPasswordChange: securityData.LastPasswordChange,
            TotalDevicesCount: securityData.TotalDevicesCount,
            RecoveryEmail: securityData.RecoveryEmail,
            UsedRecoveryCodesCount: securityData.UsedCodes,
            UnusedRecoveryCodesCount: securityData.UnusedCodes
        );

        return Result<UserSecurityDetailsResult>.Success(result, OperationStatusCode.Success);
    }
}