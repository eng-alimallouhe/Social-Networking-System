using Fido2NetLib;
using Fido2NetLib.Objects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SNS.Application.Abstractions.Caching;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.InitiatePasskeyRegistration;

public class InitiatePasskeyRegistrationCommandHandler
    : ICommandHandler<InitiatePasskeyRegistrationCommand, CredentialCreateOptions>
{
    private readonly IFido2 _fido2;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICacheService _cache;
    private readonly ICurrentUserService _currentUserService;

    public InitiatePasskeyRegistrationCommandHandler(
        IFido2 fido2,
        IApplicationDbContext dbContext,
        ICacheService cache,
        ICurrentUserService currentUserService)
    {
        _fido2 = fido2;
        _dbContext = dbContext;
        _cache = cache;
        _currentUserService = currentUserService;
    }

    public async Task<Result<CredentialCreateOptions>> Handle(
        InitiatePasskeyRegistrationCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var user = await _dbContext.Users
            .Where(u => u.Id == userId)
            .Select(u => new { u.UserSecuritySettings.RecoveryEmail, u.UserName })
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null)
        {
            return Result<CredentialCreateOptions>.Failure(UserStatusCodes.NotFound);
        }

        var existingCredentials = await _dbContext.UserPasskeys
            .Where(p => p.UserId == userId)
            .Select(p => new PublicKeyCredentialDescriptor(p.CredentialId))
            .ToListAsync(cancellationToken);

        var fidoUser = new Fido2User
        {
            Id = userId!.Value.ToByteArray(), 
            DisplayName = user.UserName,
            Name = user.UserName
        };

        var authenticatorSelection = new AuthenticatorSelection
        {
            ResidentKey = ResidentKeyRequirement.Required,
            UserVerification = UserVerificationRequirement.Required 
        };

        var attestationPreference = AttestationConveyancePreference.None;

        var parameters = new RequestNewCredentialParams
        {
            User = fidoUser,
            AuthenticatorSelection = authenticatorSelection,
            AttestationPreference = attestationPreference,
            ExcludeCredentials = existingCredentials 
        };

        var options = _fido2.RequestNewCredential(parameters);
        
        var cacheKey = $"fido2_reg_options:{userId}";

        await _cache.SetAsync(cacheKey, options, TimeSpan.FromMinutes(3)); 

        return Result< CredentialCreateOptions>.Success(options, OperationStatusCode.Success);
    }
}
