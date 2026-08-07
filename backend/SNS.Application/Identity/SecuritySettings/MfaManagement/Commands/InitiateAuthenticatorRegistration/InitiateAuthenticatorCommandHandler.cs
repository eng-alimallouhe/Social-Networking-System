using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.SecuritySettings.MfaManagement.DTOs;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Settings;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Identity.SecuritySettings.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.InitiateAuthenticatorRegistration;

public class InitiateAuthenticatorCommandHandler
    : ICommandHandler<InitiateAuthenticatorCommand, AuthenticatorSetupDto>
{
    private readonly AppSettings _appSettings;
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<UserSecuritySettings> _userSecuritySettingsRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUserCacheService _userCacheService;
    private readonly IGeneratorService _generatorService;


    public InitiateAuthenticatorCommandHandler(
        IOptions<AppSettings> options,
        IApplicationDbContext dbContext,
        IRepository<UserSecuritySettings> userSecuritySettingsRepo,
        ICurrentUserService currentUserService,
        IUserCacheService userCacheService,
        IGeneratorService generatorService)
    {
        _appSettings = options.Value;
        _dbContext = dbContext;
        _userSecuritySettingsRepo = userSecuritySettingsRepo;
        _currentUserService = currentUserService;
        _userCacheService = userCacheService;
        _generatorService = generatorService;
    }

    public async Task<Result<AuthenticatorSetupDto>> Handle(
        InitiateAuthenticatorCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        if (userId == null || userId == Guid.Empty)
        {
            return Result<AuthenticatorSetupDto>.Failure(OperationStatusCode.AuthenticationRequired);
        }

        var securitySettings = await _dbContext
            .Users
            .Where(s => s.Id == userId.Value)
            .Select(s => new 
            {
                Id = s.Id,
                UserId = s.Id,
                UserName = s.UserName,
                AuthenticatorSecretKey = s.UserSecuritySettings.AuthenticatorSecretKey,
                RecoveryEmail = s.UserSecuritySettings.RecoveryEmail
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (securitySettings == null)
        {
            return Result<AuthenticatorSetupDto>.Failure(ResourceStatusCode.NotFound);
        }

        string secretKey = _generatorService.GenerateSecureString(16);

        string? accountName = securitySettings.RecoveryEmail;

        if (string.IsNullOrWhiteSpace(accountName))
        {
            accountName = securitySettings.UserName;
        }
        
        await _userCacheService.InitiateAuthenticatorAsync(userId.Value, secretKey, cancellationToken);

        var brandName = _appSettings.BrandName ?? "SNS";
        var labelFormatted = $"{brandName}:{accountName}";

        var escapedLabel = Uri.EscapeDataString(labelFormatted);
        var escapedIssuer = Uri.EscapeDataString(brandName);
        var escapedSecret = Uri.EscapeDataString(secretKey);

        var qrCodeUri = $"otpauth://totp/{escapedLabel}?secret={escapedSecret}&issuer={escapedIssuer}&algorithm=SHA1&digits=6&period=30";

        await _userCacheService.InitiateAuthenticatorAsync(userId.Value, secretKey, cancellationToken);

        return Result<AuthenticatorSetupDto>.Success(
            new AuthenticatorSetupDto(secretKey, qrCodeUri),
            OperationStatusCode.Success);
    }
}
