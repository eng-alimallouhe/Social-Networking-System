using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Users;
using SNS.Application.Identity.Shared.DTOs.VerificationCodes;
using SNS.Application.Identity.Users.Registeration.DTOs;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Identity.Shared.Enums;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.Users.Registeration.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, RegisterResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRepository<User> _userRepo;
    private readonly ISoftDeletableRepository<Role> _roleRepo;
    private readonly ICodeService _codeService;
    private readonly IGeneratorService _generatorService;
    private readonly IHashingService _hashingService;
    private readonly IRequestInfoService _requestInfoService;
    private readonly IUrlGeneratorService _urlGeneratorService;
    private readonly IUserCacheService _userCacheService;
    private readonly IApplicationDbContext _dbContext;
    public RegisterUserCommandHandler(
        IUnitOfWork unitOfWork,
        IRepository<User> userRepo,
        ISoftDeletableRepository<Role> roleRepo,
        ICodeService codeService,
        IGeneratorService generatorService,
        IHashingService hashingService,
        IRequestInfoService requestInfoService,
        IUrlGeneratorService urlGeneratorService,
        IUserCacheService userCacheService,
        IApplicationDbContext dbContext)
    {
        _userRepo = userRepo;
        _roleRepo = roleRepo;
        _codeService = codeService;
        _unitOfWork = unitOfWork;
        _generatorService = generatorService;
        _hashingService = hashingService;
        _requestInfoService = requestInfoService;
        _urlGeneratorService = urlGeneratorService;
        _userCacheService = userCacheService;
        _dbContext = dbContext;
    }

    public async Task<Result<RegisterResponseDto>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepo.GetSingleByExpressionAsync(
            u => u.Email == request.Email, cancellationToken);

        var usersRole = await _roleRepo.GetSingleByExpressionAsync(r => r.Type == RoleType.User, cancellationToken);
        
        if (usersRole == null)
        {
            return Result<RegisterResponseDto>.Failure(OperationStatusCode.Failure);
        }

        UserSecuritySettings userSecuritySettings;
        
        bool isRecycledUser = false;

        if (user != null)
        {
            if (user.IsVerified)
            {
                var isProfileSetupCompleted = await _dbContext
                    .Profiles
                    .AnyAsync(p => p.UserId == user.Id, cancellationToken);

                if (isProfileSetupCompleted)
                    return Result<RegisterResponseDto>.Failure(UserStatusCodes.AlreadyExists);

                return Result<RegisterResponseDto>.Failure(new RegisterResponseDto(user.Id, IsProfileCompleted: false), UserStatusCodes.ProfileNotCompleted);
            }

            user.ChangePassword(hashedPassword: _hashingService.Hash(request.Password));
            
            user.ChangePreferredLanguage(language: _requestInfoService.Language);

            userSecuritySettings = user.UserSecuritySettings;
            
            isRecycledUser = true;
        }
        else
        {
            var username = await GenerateUniqueUsernameAsync(cancellationToken);

            user = User.Create(
                roleId: usersRole.Id,
                userName: username,
                email: request.Email,
                passwordHash: _hashingService.Hash(request.Password)
            );
            user.ChangePreferredLanguage(language: _requestInfoService.Language);

            userSecuritySettings = UserSecuritySettings.Create(userId: user.Id, recoveryEmail: null);
            user.SetSecuritySettings(settings: userSecuritySettings);
        }

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            if (!isRecycledUser)
            {
                _userRepo.Add(user);
            }

            var token = _generatorService.GenerateSecureString();
            var redirectUrl = _urlGeneratorService.GenerateAccountActivationUrl(user.Id, token);

            var sendCodeDto = new CodeSendRequest(
                UserId: user.Id,
                UserName: user.UserName,
                RecipientAddress: user.Email,
                Purpose: SendPurpose.UserVerification,
                SendMethod: CommunicationMethod.Email,
                SendLanguage: user.PreferredLanguage,
                RedirectUrl: redirectUrl,
                Token: token);

            var codeSendResult = await _codeService.SendCodeAsync(sendCodeDto, cancellationToken);

            if (codeSendResult.IsFailure)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<RegisterResponseDto>.Failure(codeSendResult.StatusCode);
            }

            await _unitOfWork.CompleteAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            await _userCacheService.SetUserAsync(new UserModel(
                UserId: user.Id,
                UserName: user.UserName,
                RoleId: user.RoleId,
                Email: user.Email,
                RoleType: usersRole.Type,
                RecoveryEmail: null,
                CommunicationMethod: userSecuritySettings?.DefaultCommunicationMethod ?? CommunicationMethod.Email,
                PreferredLanguage: user.PreferredLanguage,
                Status: user.Status), cancellationToken);

            var successStatusCode = isRecycledUser ? UserStatusCodes.NotVerified : VerificationStatusCodes.CodeSent;

            return Result<RegisterResponseDto>.Success(
                new RegisterResponseDto(UserId: user.Id, Token: token),
                successStatusCode);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task<string> GenerateUniqueUsernameAsync(CancellationToken cancellationToken)
    {
        string[] allowedNames =
        {
            "member", "spark", "persona", "connector", "nova",
            "atlas", "node", "circle", "echo", "pulse", "horizon"
        };

        var randomName = allowedNames[Random.Shared.Next(allowedNames.Length)];
        var randomSuffix = _generatorService.GenerateSecureCode().Substring(0, 4);
        var username = $"{randomName}{randomSuffix}";

        while (await _userRepo.ExistsAsync(u => u.UserName == username, cancellationToken))
        {
            randomSuffix = _generatorService.GenerateSecureCode().Substring(0, 4);
            username = $"{randomName}{randomSuffix}";
        }

        return username;
    }
}