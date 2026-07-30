using Fido2NetLib;
using Fido2NetLib.Objects;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SNS.Application.Abstractions.Caching;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Identity.SecuritySettings.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.CompletePasskeyRegistration;

/// <summary>
/// Handles the execution of <see cref="CompletePasskeyRegistrationCommand"/> to complete FIDO2 passkey registration.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Retrieves cached registration options for the current user and invalidates cache.
/// 2. Verifies the authenticator attestation response and credential uniqueness via FIDO2.
/// 3. Creates a new passkey record and updates user security settings to enable MFA with Passkey.
/// 4. Persists passkey and security setting changes in database.
/// Side effects include cache eviction, passkey creation, and updating user MFA settings.
/// </remarks>
public class CompletePasskeyRegistrationCommandHandler
    : ICommandHandler<CompletePasskeyRegistrationCommand, Unit>
{
    private readonly IRepository<UserPasskey> _userPasskeyRepo;
    private readonly IRepository<UserSecuritySettings> _userSecuritySettingsRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFido2 _fido2;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICacheService _cache;
    private readonly ICurrentUserService _currentUserService;

    public CompletePasskeyRegistrationCommandHandler(
        IRepository<UserPasskey> userPasskeyRepo,
        IRepository<UserSecuritySettings> userSecuritySettingsRepo,
        IUnitOfWork unitOfWork,
        IFido2 fido2,
        IApplicationDbContext dbContext,
        ICacheService cache,
        ICurrentUserService currentUserService)
    {
        _userPasskeyRepo = userPasskeyRepo;
        _userSecuritySettingsRepo = userSecuritySettingsRepo;
        _unitOfWork = unitOfWork;
        _fido2 = fido2;
        _dbContext = dbContext;
        _cache = cache;
        _currentUserService = currentUserService;
    }

    public async Task<Result<Unit>> Handle(
        CompletePasskeyRegistrationCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null || userId == Guid.Empty)
        {
            return Result<Unit>.Failure(UserStatusCodes.NotFound);
        }

        var cacheKey = $"fido2_reg_options:{userId.Value}";
        CredentialCreateOptions? origOptions = await _cache.GetAsync<CredentialCreateOptions>(cacheKey, cancellationToken);

        if (origOptions == null)
        {
            return Result<Unit>.Failure(SessionStatusCodes.NotFound);
        }

        await _cache.RemoveAsync(cacheKey, cancellationToken);

        try
        {
            var makeNewCredentialParams = new MakeNewCredentialParams
            {
                AttestationResponse = request.AttestationResponse,
                OriginalOptions = origOptions,
                IsCredentialIdUniqueToUserCallback = async (args, cancellation) =>
                {
                    bool exists = await _dbContext.UserPasskeys
                        .AnyAsync(p => p.CredentialId == args.CredentialId, cancellation);
                    return !exists;
                }
            };

            RegisteredPublicKeyCredential fidoCredential = await _fido2.MakeNewCredentialAsync(
                makeNewCredentialParams,
                cancellationToken: cancellationToken
            );

            if (fidoCredential == null)
            {
                return Result<Unit>.Failure(SecurityStatusCodes.VerificationFailed);
            }

            var newPasskey = UserPasskey.Create(
                userId: userId.Value,
                credentialId: fidoCredential.Id,
                publicKey: fidoCredential.PublicKey,
                deviceName: request.DeviceName,
                signatureCounter: fidoCredential.SignCount
            );

            _userPasskeyRepo.Add(newPasskey);

            var securitySettings = await _userSecuritySettingsRepo
                .GetSingleByExpressionAsync(s => s.UserId == userId.Value, cancellationToken);

            if (securitySettings != null)
            {
                securitySettings.EnableMfa(mfaProvider: MfaProvider.Passkey);
            }

            await _unitOfWork.CompleteAsync(cancellationToken);

            return Result<Unit>.Success(Unit.Value, OperationStatusCode.Success);
        }
        catch (Fido2VerificationException)
        {
            return Result<Unit>.Failure(SecurityStatusCodes.VerificationFailed);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
