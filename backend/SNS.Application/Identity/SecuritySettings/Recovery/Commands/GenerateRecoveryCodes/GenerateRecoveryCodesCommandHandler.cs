using SNS.Application.Abstractions.Common;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using Microsoft.EntityFrameworkCore;

namespace SNS.Application.Identity.SecuritySettings.Recovery.Commands.GenerateRecoveryCodes;

public sealed class GenerateRecoveryCodesCommandHandler
    : ICommandHandler<GenerateRecoveryCodesCommand, IReadOnlyCollection<string>>
{
    private readonly IApplicationDbContext _dbContext; 
    private readonly IRepository<RecoveryCode> _recoveryCodeRepo; 
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;
    private readonly IGeneratorService _generatorService; 

    public GenerateRecoveryCodesCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<RecoveryCode> recoveryCodeRepo,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService,
        IGeneratorService generatorService)
    {
        _dbContext = dbContext;
        _recoveryCodeRepo = recoveryCodeRepo;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
        _generatorService = generatorService;
    }

    public async Task<Result<IReadOnlyCollection<string>>> Handle(
        GenerateRecoveryCodesCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        if (userId == null || userId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<string>>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        // 1️⃣ جلب إعدادات الأمان الخاصة بالمستخدم (قراءة طاهرة عبر الـ DbContext)
        var securitySettingsId = await _dbContext.UsersSecuritySettings
            .Where(ss => ss.UserId == userId)
            .Select(ss => ss.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (securitySettingsId == Guid.Empty)
        {
            return Result<IReadOnlyCollection<string>>.Failure(ResourceStatusCode.NotFound);
        }

        // 2️⃣ التحقق من شرطك الصارم 🛡️: هل توجد أكواد حالية غير مستخدمة؟
        var hasActiveCodes = await _dbContext.RecoveryCodes
            .AnyAsync(rc => rc.UserSecuritySettingsId == securitySettingsId && !rc.IsUsed, cancellationToken);

        if (hasActiveCodes)
        {
            return Result<IReadOnlyCollection<string>>.Failure(SecurityStatusCodes.RequestRejected);
        }

        var plainCodes = new List<string>();

        for (int i = 0; i < 6; i++)
        {
            string plainCode = _generatorService.GenerateSecretKey()[..8];
            plainCodes.Add(plainCode);

            string hashedCode = BCrypt.Net.BCrypt.HashPassword(plainCode, workFactor: 12);

            var recoveryCodeEntity = RecoveryCode.Create(securitySettingsId, hashedCode);

            await _recoveryCodeRepo.AddAsync(recoveryCodeEntity, cancellationToken);
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<IReadOnlyCollection<string>>.Success(plainCodes.AsReadOnly(), OperationStatusCode.Success);
    }
}