using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public InitiateAuthenticatorCommandHandler(
        IOptions<AppSettings> options,
        IApplicationDbContext dbContext,
        IRepository<UserSecuritySettings> userSecuritySettingsRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _appSettings = options.Value;
        _dbContext = dbContext;
        _userSecuritySettingsRepo = userSecuritySettingsRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
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

        var securitySettings = await _userSecuritySettingsRepo
            .GetSingleByExpressionAsync(s => s.UserId == userId.Value, cancellationToken);

        if (securitySettings == null)
        {
            return Result<AuthenticatorSetupDto>.Failure(ResourceStatusCode.NotFound);
        }

        // ??? ????? ?????: ??? ??? ??????? ?????? ??????? ??????? ???? ????? ????? ??? Secret ????????
        if (securitySettings.MfaProvider == MfaProvider.AuthenticatorApp)
        {
            return Result<AuthenticatorSetupDto>.Failure(SecurityStatusCodes.MfaAlreadyEnabled);
        }

        string secretKey = securitySettings.InitiateAuthenticatorSetup();

        string? accountName = securitySettings.RecoveryEmail;

        if (string.IsNullOrWhiteSpace(accountName))
        {
            var userName = await _dbContext.Users
                .Where(u => u.Id == userId.Value)
                .Select(s => s.UserName)
                .FirstOrDefaultAsync(cancellationToken);

            if (userName == null)
                return Result<AuthenticatorSetupDto>.Failure(UserStatusCodes.NotFound);

            accountName = userName;
        }

        var brandName = _appSettings.BrandName ?? "SNS";
        var labelFormatted = $"{brandName}:{accountName}";

        var escapedLabel = Uri.EscapeDataString(labelFormatted);
        var escapedIssuer = Uri.EscapeDataString(brandName);
        var escapedSecret = Uri.EscapeDataString(secretKey);

        var qrCodeUri = $"otpauth://totp/{escapedLabel}?secret={escapedSecret}&issuer={escapedIssuer}&algorithm=SHA1&digits=6&period=30";

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<AuthenticatorSetupDto>.Success(
            new AuthenticatorSetupDto(secretKey, qrCodeUri),
            OperationStatusCode.Success);
    }
}
