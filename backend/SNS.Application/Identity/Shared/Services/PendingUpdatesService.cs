using SNS.Application.Abstractions.Caching;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.PendingUpdates;
using SNS.Domain.Identity.Users.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Identity.Shared.Services;

public class PendingUpdatesService : IPendingUpdatesService
{
    private readonly IIdentityCacheKeyFactory _identityCacheKeyFactory;
    private readonly ICacheService _cacheService;

    private readonly TimeSpan _updateTTL = TimeSpan.FromMinutes(15);

    public PendingUpdatesService(
        ICacheService cacheService,
        IIdentityCacheKeyFactory identityCacheKeyFactory)
    {
        _identityCacheKeyFactory = identityCacheKeyFactory;
        _cacheService = cacheService;
    }

    // =====================================================================================
    // 📧 RecoveryEmail Update Methods
    // =====================================================================================
    public async Task<Result> CreateEmailUpdateAsync(CreateEmailUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var key = _identityCacheKeyFactory.GetUpdateKey(dto.UserId, UpdateType.Email);

        var updateModel = new EmailUpdateModel(dto.NewEmail, dto.Token);

        await _cacheService.SetAsync(key, updateModel, _updateTTL, cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }

    public async Task<EmailUpdateModel?> GetEmailUpdateAsync(Guid userId, CancellationToken cancellationToken)
    {
        var key = _identityCacheKeyFactory.GetUpdateKey(userId, UpdateType.Email);
        return await _cacheService.GetAsync<EmailUpdateModel>(key, cancellationToken);
    }

    public async Task<Result> DeleteEmailUpdateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var key = _identityCacheKeyFactory.GetUpdateKey(userId, UpdateType.Email);
        await _cacheService.RemoveAsync(key, cancellationToken);
        return Result.Success(OperationStatusCode.Success);
    }

    
    // =====================================================================================
    // 🔒 Password Reset/Update Methods
    // =====================================================================================
    public async Task<Result> CreatePasswordUpdateAsync(CreatePasswordUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var key = _identityCacheKeyFactory.GetUpdateKey(dto.UserId, UpdateType.Password);

        var updateModel = new PasswordUpdateModel(dto.Token);

        await _cacheService.SetAsync(key, updateModel, _updateTTL, cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }

    public async Task<PasswordUpdateModel?> GetPasswordUpdateAsync(Guid userId, CancellationToken cancellationToken)
    {
        var key = _identityCacheKeyFactory.GetUpdateKey(userId, UpdateType.Password);
        return await _cacheService.GetAsync<PasswordUpdateModel>(key, cancellationToken);
    }

    public async Task<Result> DeletePasswordUpdateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var key = _identityCacheKeyFactory.GetUpdateKey(userId, UpdateType.Password);
        await _cacheService.RemoveAsync(key, cancellationToken);
        return Result.Success(OperationStatusCode.Success);
    }

    public async Task<Result> ConfirmPasswordUpdateAsync(VerifiedPasswordUpdateDto dto, CancellationToken cancellationToken = default)
    {
        var key = _identityCacheKeyFactory.GetUpdateKey(dto.UserId, UpdateType.Password);

        var remainingTTL = await _cacheService.GetKeyTTLAsync(key, cancellationToken);

        if (remainingTTL <= TimeSpan.Zero)
        {
            return Result.Failure(OperationStatusCode.ExpiredInfo);
        }

        var updateModel = new PasswordUpdateModel(
            Token: dto.Token,
            IsVerified: dto.IsVerified);

        await _cacheService.SetAsync(key, updateModel, remainingTTL, cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}