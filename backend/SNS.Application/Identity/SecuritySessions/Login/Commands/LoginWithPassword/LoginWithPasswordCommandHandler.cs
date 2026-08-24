using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.ArchiveManagement.Abstractions;
using SNS.Application.Identity.SecuritySessions.Login.Contracts;
using SNS.Application.Identity.SecuritySessions.SessionsManagement.Contracts;
using SNS.Application.Identity.SecuritySessions.Shared.Abstractions;
using SNS.Application.Identity.SecuritySessions.Shared.Contracts;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Authentication;
using SNS.Application.Identity.Shared.DTOs.SecuritySessions;
using SNS.Application.Identity.Shared.DTOs.VerificationCodes;
using SNS.Application.Identity.Shared.ValueObjects;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.SecuritySessions.Entities;
using SNS.Domain.Identity.SecuritySessions.Events;
using SNS.Domain.Identity.SecuritySettings.Enums;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Identity.Users.Events;
using SNS.Domain.Identity.Users.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.SecuritySessions.Login.Commands.LoginWithPassword;

/// <summary>
/// Handles the execution of <see cref="LoginWithPasswordCommand"/> to authenticate a user using password credentials.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Verifies user existence, lockouts, suspension, ban status, and password hash.
/// 2. Handles MFA workflows if enabled (TOTP, Email, SMS, Passkey challenge).
/// 3. Registers or updates device information and starts a new security session upon successful verification.
/// 4. Generates JWT access and refresh tokens.
/// 5. Logs login activity into the user archive and publishes domain events (such as <see cref="UserLoggedInEvent"/>).
/// Side effects include updating login attempt counts, session persistence, device registration, audit logging, and event publishing.
/// </remarks>
public sealed class LoginWithPasswordCommandHandler : ICommandHandler<LoginWithPasswordCommand, LoginInitialResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationDbContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly IRequestInfoService _requestInfoService;
    private readonly IRepository<User> _userRepo;
    private readonly IHashingService _hashingService;
    private readonly ICodeService _codeService;
    private readonly IArchiveService _archiveService;
    private readonly ISessionService _sessionService;
    private readonly IDeviceService _deviceService;
    private readonly IGeneratorService _generatorService;
    private readonly IUrlGeneratorService _urlGenerator;
    private readonly IUserCacheService _userCacheService;

    public LoginWithPasswordCommandHandler(
        IUnitOfWork unitOfWork,
        IApplicationDbContext dbContext,
        ITokenService tokenService,
        IRepository<User> userRepo,
        IHashingService hashingService,
        ICodeService codeService,
        IUrlGeneratorService urlGenerator,
        IUserCacheService userCacheService,
        IGeneratorService generatorService,
        IArchiveService archiveService,
        IRequestInfoService requestInfoService,
        ISessionService sessionService,
        IDeviceService deviceService)
    {
        _unitOfWork = unitOfWork;
        _dbContext = dbContext;
        _tokenService = tokenService;
        _userRepo = userRepo;
        _sessionService = sessionService;
        _hashingService = hashingService;
        _codeService = codeService;
        _archiveService = archiveService;
        _requestInfoService = requestInfoService;
        _urlGenerator = urlGenerator;
        _userCacheService = userCacheService;
        _deviceService = deviceService;
        _generatorService = generatorService;
    }

    public async Task<Result<LoginInitialResponseDto>> Handle(LoginWithPasswordCommand request, CancellationToken cancellationToken)
    {
        var identifier = new UserIdentifier(request.Identifier);

        var spec = new UserForLoginSpecification(request.Identifier, identifier.Type);

        var user = await _userRepo.GetSingleAsync(spec, cancellationToken);

        if (user == null)
        {
            await Task.Delay(Random.Shared.Next(100, 300), cancellationToken);
            return Result<LoginInitialResponseDto>.Failure(UserStatusCodes.NotFound);
        }

        if (user.Status == UserStatus.PermanentlyBanned)
        {
            return Result<LoginInitialResponseDto>.Failure(UserStatusCodes.Banned);
        }

        if (user.Status == UserStatus.Deactivated)
        {
            return Result<LoginInitialResponseDto>.Failure(UserStatusCodes.Banned);
        }

        if (user.Status == UserStatus.Suspended && user.SuspendedUntil > DateTime.UtcNow)
        {
            return Result<LoginInitialResponseDto>.Failure(
                new LoginInitialResponseDto(SuspendedUntil: user.SuspendedUntil, SuspensionReason: user.SuspensionReason), 
                UserStatusCodes.Suspended);
        }

        var requestInformationModel = new RequestInformationModel(
            IpAddress: _requestInfoService.IpAddress,
            Country: _requestInfoService.Country,
            DeviceName: _requestInfoService.DeviceName,
            Browser: _requestInfoService.Browser,
            Longitude: _requestInfoService.Longitude,
            Latitude: _requestInfoService.Latitude,
            City: _requestInfoService.City,
            DeviceId: _requestInfoService.DeviceId,
            FingerprintHash: _requestInfoService.FingerprintHash,
            OperatingSystem: _requestInfoService.OperatingSystem,
            DeviceToken: _requestInfoService.DeviceToken,
            DeviceModel: _requestInfoService.DeviceModel,
            DeviceVendor: _requestInfoService.DeviceVendor);

        // detect the target send (email or sms) for sending notifications about the login attempt based on the user's security settings and available contact information, then include that information in the relevant domain events so that the notification service can use it to send alerts to the user about important account activities such as successful logins from new devices or locations, failed login attempts, password changes, or other security-related events. This helps keep users informed about their account activity and can alert them to potential unauthorized access.
        var recipientAddress = user.UserSecuritySettings.DefaultCommunicationMethod switch
        {
            CommunicationMethod.RecoveryEmail when !string.IsNullOrEmpty(user.UserSecuritySettings.RecoveryEmail) => user.UserSecuritySettings.RecoveryEmail,
            CommunicationMethod.Email when !string.IsNullOrEmpty(user.Email) => user.Email,
            _ => user.Email
        };

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            bool isPasswordValid = _hashingService.Verify(request.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                user.IncrementFailedLoginAttempts();

                // Handle max failed login attempts and potential account lockout:
                if (user.FailedLoginAttempts > 5)
                {
                    user.Suspend(
                        suspendedUntil: DateTime.UtcNow.AddMinutes(15),
                        reason: UserStatusCodes.MaxLoginAttempts.ToString());

                    if (user.UserSecuritySettings.FailedLoginNotifications)
                    {
                        user.AddDomainEvent(new UserSuspendedEvent(
                            UserId: user.Id,
                            UserName: user.UserName,
                            SendLanguage: user.PreferredLanguage,
                            RecipientAddress: recipientAddress,
                            SendMethod: user.UserSecuritySettings.DefaultCommunicationMethod,
                            SuspensionReason: UserStatusCodes.MaxLoginAttempts.ToString(),
                            IpAddress: requestInformationModel.IpAddress,
                            DeviceName: requestInformationModel.DeviceName,
                            Longitude: requestInformationModel.Longitude,
                            Latitude: requestInformationModel.Latitude,
                            Country: requestInformationModel.Country,
                            City: requestInformationModel.City,
                            OccurredOn: DateTime.UtcNow));
                    }

                    await _unitOfWork.CompleteAsync(cancellationToken);

                    await _unitOfWork.CommitTransactionAsync(cancellationToken);

                    await _userCacheService.RemoveUserAsync(user.Id, cancellationToken);

                    return Result<LoginInitialResponseDto>.Failure(
                        new LoginInitialResponseDto(
                            SuspendedUntil: user.SuspendedUntil,
                            SuspensionReason: user.SuspensionReason),
                        UserStatusCodes.LockedOut);
                }

                // Handle user notification for failed login attempt if enabled in security settings
                if (user.UserSecuritySettings.FailedLoginNotifications)
                {
                    user.AddDomainEvent(new UserFailedToLoginEvent(
                        UserId: user.Id,
                        UserName: user.UserName,
                        RecipientAddress: recipientAddress,
                        SendMethod: user.UserSecuritySettings.DefaultCommunicationMethod,
                        IpAddress: requestInformationModel.IpAddress,
                        SendLanguage: user.PreferredLanguage,
                        Device: requestInformationModel.DeviceName,
                        Longitude: requestInformationModel.Longitude,
                        Latitude: requestInformationModel.Latitude,
                        Country: requestInformationModel.Country,
                        City: requestInformationModel.City,
                        OccurredOn: DateTime.UtcNow));
                }

                // Consider adding a short delay here to mitigate brute-force attacks, but be cautious of potential abuse for DoS attacks. Implementing an exponential backoff strategy could be more effective.
                await Task.Delay(Random.Shared.Next(100, 300), cancellationToken);

                await _unitOfWork.CompleteAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return Result<LoginInitialResponseDto>.Failure(UserStatusCodes.FailedLoginAttempt);
            }

            if (user.UserProfile == null)
            {
                var result = await HandleLowRiskLoginAsync(user, requestInformationModel, recipientAddress, cancellationToken);
                if (result.IsFailure)
                {
                    await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                }
                else
                {
                    await _unitOfWork.CompleteAsync(cancellationToken);
                    await _unitOfWork.CommitTransactionAsync(cancellationToken);
                }
                return result;
            }

            // Calculate risk score based on various factors such as IP address, device fingerprint, geolocation, and historical login patterns. This can help determine if additional verification steps are needed or if the login attempt should be blocked.
            var riskScore = await CheckLoginRiskScoreAsync(
                user: user,requestInformationModel: requestInformationModel, cancellationToken: cancellationToken);


            switch (riskScore)
            {
                //Low Risk, normal login, just check if the user has enabled any additional security measures like 2FA and proceed accordingly
                case <= 25:
                    var lowRiskHandlerResult = await HandleLowRiskLoginAsync(user, requestInformationModel, recipientAddress, cancellationToken);
                    if (lowRiskHandlerResult.IsFailure)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    }
                    else
                    {
                        await _unitOfWork.CompleteAsync(cancellationToken);
                        await _unitOfWork.CommitTransactionAsync(cancellationToken);
                    }
                    return lowRiskHandlerResult;

                //Medium Risk, require TFA only
                case > 25 and <= 50:
                    var mediumRiskHandlerResult = await HandleMediumRiskLoginAsync(user, recipientAddress, cancellationToken);
                    if (mediumRiskHandlerResult.IsFailure)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    }
                    else
                    {
                        await _unitOfWork.CompleteAsync(cancellationToken);
                        await _unitOfWork.CommitTransactionAsync(cancellationToken);
                    }
                    return mediumRiskHandlerResult;

                //High Risk, force TFA and alert user by sending email or sms
                case > 50 and <= 85:
                    var highRiskHandlerResult = await HandleHighRiskLoginAsync(user, requestInformationModel, recipientAddress, cancellationToken);
                    if (highRiskHandlerResult.IsFailure)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    }
                    else
                    {
                        await _unitOfWork.CompleteAsync(cancellationToken);
                        await _unitOfWork.CommitTransactionAsync(cancellationToken);
                    }
                    return highRiskHandlerResult;

                //Critical Risk, Block the login attempt, suspend account for a longer period, and require user to contact support to verify identity and unlock account, also send high priority alert to user about the blocked login attempt and recommend enabling additional security measures then enable TFA if not already enabled
                case > 85:
                    var criticalRiskHandlerResult = await HandleCriticalRiskLoginAsync(user, recipientAddress, cancellationToken);
                    if (criticalRiskHandlerResult.IsFailure)
                    {
                        await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                    }
                    else
                    {
                        await _unitOfWork.CompleteAsync(cancellationToken);
                        await _unitOfWork.CommitTransactionAsync(cancellationToken);
                    }
                    return criticalRiskHandlerResult;
            }

        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }


    private async Task<int> CheckLoginRiskScoreAsync(
    User user,
    RequestInformationModel requestInformationModel,
    CancellationToken cancellationToken)
    {
        var recentSessions = await _dbContext.UserSessions
            .Where(s => 
            s.UserId == user.Id && !s.IsRevoked)
            .OrderByDescending(s => s.LastSeenAt)
            .Take(10)
            .Select(s => new SessionSnapshotDto(
                Id: s.Id,
                UserId: s.UserId,
                DeviceId: s.DeviceId,
                DeviceFingerprintHash: s.Device.FingerprintHash,
                DeviceToken: s.Device.DeviceToken,
                Browser: s.Device.Browser,
                OS: s.Device.OperatingSystem,
                Country: s.Country,
                Longitude: s.Longitude,
                Latitude: s.Latitude,
                LoginAt: s.LoginAt,
                LogoutAt: s.LogoutAt,
                LastSeenAt: s.LastSeenAt,
                IsDeviceTrusted: s.Device.IsTrusted))
            .ToListAsync(cancellationToken);

        int score = 0;

        if (!recentSessions.Any())
        {
            return 0;
        }

        var knownCountry =
            recentSessions.Any(s => s.Country == requestInformationModel.Country);

        var knownDevice =
            recentSessions.Any(s => s.DeviceFingerprintHash == requestInformationModel.FingerprintHash);

        var trustedFingerprintMatch = recentSessions.Any(s => s.IsDeviceTrusted && s.DeviceFingerprintHash == requestInformationModel.FingerprintHash);

        var knownBrowser =
            recentSessions.Any(s => s.Browser == requestInformationModel.Browser);

        var lastSession = recentSessions
            .OrderByDescending(s => s.LastSeenAt)
            .First();

        var distanceMeters = CalculateDistance(
            lastSession.Latitude,
            lastSession.Longitude,
            requestInformationModel.Latitude,
            requestInformationModel.Longitude);

        var distanceKm = distanceMeters / 1000.0;

        var hours =
            (DateTime.UtcNow - lastSession.LastSeenAt).TotalHours;

        if (hours <= 0)
            hours = 0.1;

        var speed = distanceKm / hours;

        if (!knownCountry)
            score += 10;

        if (!knownBrowser)
            score += 5;

        if (!knownDevice)
            score += 35;

        if (distanceKm > 500)
            score += 10;

        if (speed > 900)
            score += 40;

        if (knownDevice)
            score -= 25;

        if (trustedFingerprintMatch)
            score -= 40;

        return Math.Max(score, 0) * 0;
    }

    private double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        var R = 6371; // Radius of the Earth in kilometers
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a =
            Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
            Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
            Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c * 1000; // Distance in meters
    }

    private double ToRadians(double angle)
    {
        return angle * (Math.PI / 180);
    }


    private async Task<Result<LoginInitialResponseDto>> HandleCriticalRiskLoginAsync(
        User user,
        string recipientAddress,
        CancellationToken cancellationToken)
    {
        user.Suspend(
            DateTime.UtcNow.AddHours(2),
            "CriticalLoginRiskDetected");

        user.AddDomainEvent(new UserSuspendedEvent(
                UserId: user.Id,
                UserName: user.UserName,
                RecipientAddress: recipientAddress,
                SendLanguage: user.PreferredLanguage,
                SendMethod: user.UserSecuritySettings.DefaultCommunicationMethod,
                SuspensionReason: "Critical Login Risk Attempt Blocked",
                IpAddress: _requestInfoService.IpAddress,
                DeviceName: _requestInfoService.DeviceName,
                Longitude: _requestInfoService.Longitude,
                Latitude: _requestInfoService.Latitude,
                Country: _requestInfoService.Country,
                City: _requestInfoService.City,
                OccurredOn: DateTime.UtcNow));

        await _userCacheService.RemoveUserAsync(user.Id, cancellationToken);

        return Result<LoginInitialResponseDto>.Failure(new LoginInitialResponseDto(
            SuspendedUntil: user.SuspendedUntil,
            SuspensionReasonCode: SecurityStatusCodes.CriticalLoginRisk), UserStatusCodes.Suspended);
    }

    private async Task<Result<LoginInitialResponseDto>> HandleHighRiskLoginAsync(
        User user,
        RequestInformationModel reqInfo,
        string recipientAddress,
        CancellationToken cancellationToken)
    {
        user.AddDomainEvent(new HighRiskLoginDetectedEvent(
            UserId: user.Id,
            UserName: user.UserName,
            IpAddress: reqInfo.IpAddress,
            Country: reqInfo.Country,
            City: reqInfo.City,
            Device: reqInfo.DeviceName,
            SendMethod: user.UserSecuritySettings.DefaultCommunicationMethod,
            RecipientAddress: recipientAddress,
            SendLanguage: user.PreferredLanguage,
            OccurredOn: DateTime.UtcNow,
            Longitude: reqInfo.Longitude,
            Latitude: reqInfo.Latitude));

        // تصعيد التدفق لتوليد تحدي الـ MFA حتماً مع الحفاظ على مطابقة الأنواع (Result<LoginInitialResponseDto>)
        return await HandleMediumRiskLoginAsync(user, recipientAddress, cancellationToken);
    }

    private async Task<Result<LoginInitialResponseDto>> HandleMediumRiskLoginAsync(
        User user,
        string recipientAddress,
        CancellationToken cancellationToken)
    {
        if (user.UserSecuritySettings.MfaProvider == MfaProvider.AuthenticatorApp)
        {
            return Result<LoginInitialResponseDto>.Success(
                new LoginInitialResponseDto(
                    IsMfaRequired: true,
                    MfaProviderType: MfaProvider.AuthenticatorApp),
                SecurityStatusCodes.TfaRequired);
        }

        var token = _generatorService.GenerateSecureString();
        var redirectUrl = _urlGenerator.GenerateTFARedirectUrl(user.Id, token);

        CommunicationMethod targetMethod;
        
        string targetAddress;

        switch (user.UserSecuritySettings.MfaProvider)
        {
            case MfaProvider.RecoveryEmail:
                targetMethod = CommunicationMethod.RecoveryEmail;
                targetAddress = user.UserSecuritySettings.RecoveryEmail ?? recipientAddress;
                break;

            case MfaProvider.Email:
            case MfaProvider.None:
            default:
                targetMethod = CommunicationMethod.Email;
                targetAddress = !string.IsNullOrEmpty(user.Email) ? user.Email : recipientAddress;
                break;
        }

        var codeRequest = new CodeSendRequest(
            UserId: user.Id,
            UserName: user.UserName,
            RecipientAddress: targetAddress,
            Purpose: SendPurpose.LoginTwoFactor,
            SendMethod: targetMethod,
            SendLanguage: user.PreferredLanguage,
            RedirectUrl: redirectUrl,
            Token: token);

        var sendResult = await _codeService.SendCodeAsync(codeRequest, cancellationToken);

        if (sendResult.IsFailure)
        {
            return Result<LoginInitialResponseDto>.Failure(sendResult.StatusCode);
        }

        return Result<LoginInitialResponseDto>.Success(
            new LoginInitialResponseDto(
                IsMfaRequired: true,
                UserId: user.Id,
                ChallengeToken: token,
                MfaProviderType: user.UserSecuritySettings.MfaProvider),
            SecurityStatusCodes.TfaRequired);
    }

    private async Task<Result<LoginInitialResponseDto>> HandleLowRiskLoginAsync(
        User user,
        RequestInformationModel requestInformationModel,
        string recipientAddress,
        CancellationToken cancellationToken)
    {
        if (user.UserSecuritySettings.IsMfaEnabled)
        {
            return await HandleMediumRiskLoginAsync(user, recipientAddress, cancellationToken);
        }

        var device = await _deviceService.GetOrCreateUserDeviceAsync(new DeviceCreateDto(
            UserId: user.Id,
            FriendlyName: requestInformationModel.DeviceName,
            FingerprintHash: requestInformationModel.FingerprintHash,
            DeviceToken: requestInformationModel.DeviceToken,
            Browser: requestInformationModel.Browser,
            OperatingSystem: requestInformationModel.OperatingSystem,
            DeviceModel: requestInformationModel.DeviceModel,
            DeviceVendor: requestInformationModel.DeviceVendor,
            IsTrusted: false));

        var sessionResult = await _sessionService.CreateSessionAsync(new CreateSessionDto(
            UserId: user.Id,
            DeviceId: device.deviceId,
            IpAddress: requestInformationModel.IpAddress,
            City: requestInformationModel.City,
            Country: requestInformationModel.Country,
            FingerprintHash: requestInformationModel.FingerprintHash,
            Browser: requestInformationModel.Browser,
            Longitude: requestInformationModel.Longitude,
            Latitude: requestInformationModel.Latitude,
            IsDeviceTrusted: device.isDeviceTrusted));

        if (sessionResult.IsFailure || sessionResult.Value == null)
        {
            return Result<LoginInitialResponseDto>.Failure(sessionResult.StatusCode);
        }

        user.AddDomainEvent(new UserLoggedInEvent(
            UserId: user.Id,
            SessionId: sessionResult.Value.SessionId,
            IpAddress: requestInformationModel.IpAddress,
            Device: requestInformationModel.DeviceName,
            City: requestInformationModel.City,
            Country: requestInformationModel.Country,
            Latitude: requestInformationModel.Latitude,
            Longitude: requestInformationModel.Longitude,
            UserLanguage: user.PreferredLanguage,
            RecipientAddress: recipientAddress,
            SendMethod: user.UserSecuritySettings.DefaultCommunicationMethod,
            OccurredOn: DateTime.UtcNow));

        var accessToken = _tokenService.GenerateAccessToken(new AccessTokenCreateDto(
            UserId: user.Id,
            ProfileId: user.UserProfile != null? user.UserProfile.Id : null,
            SessionId: sessionResult.Value.SessionId,
            RoleType: user.Role.Type));

        if (accessToken == null)
        {
            return Result<LoginInitialResponseDto>.Failure(SecurityStatusCodes.TokenGenerationError);
        }

        return Result<LoginInitialResponseDto>.Success(new LoginInitialResponseDto(
            UserId: user.Id,
            DeviceId: device.deviceId,
            AccessToken: accessToken,
            RefreshToken: sessionResult.Value.RefreshToken),
            OperationStatusCode.Success);
    }
}