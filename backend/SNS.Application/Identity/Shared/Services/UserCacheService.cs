using SNS.Application.Abstractions.Caching;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Users;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.Users.Entities;
using SNS.Domain.Identity.Users.Specifications;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;


namespace SNS.Application.Identity.Shared.Services;

public class UserCacheService : IUserCacheService
{
    private readonly IIdentityCacheKeyFactory _identityCacheKeyFactory;
    private readonly ICacheService _cacheService;
    private readonly IRepository<User> _userRepo;
    private readonly IApplicationDbContext _dbContext;

    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(20);

    public UserCacheService(
        IIdentityCacheKeyFactory identityCacheKeyFactory,
        ICacheService cacheService,
        IRepository<User> userRepo,
        IApplicationDbContext dbContext)
    {
        _identityCacheKeyFactory = identityCacheKeyFactory;
        _cacheService = cacheService;
        _userRepo = userRepo;
        _dbContext = dbContext;
    }

    public async Task<UserModel?> GetUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var key = _identityCacheKeyFactory.GetUserKey(userId);
        var model = await _cacheService.GetAsync<UserModel>(key, cancellationToken);

        if (model != null)
            return model;

        var spec = new UserWithRoleAndSettingsSpecification(userId);
        var fallBackUser = await _userRepo.GetSingleAsync(spec, cancellationToken);

        var fallBackUserModel = await _dbContext
                .Users
                .Where(u => u.Id == userId)
                .Select(u => new UserModel(
                    UserId: u.Id, UserName: u.UserName, RoleId: u.RoleId, Email: u.Email,
                    RoleType: u.Role.Type, RecoveryEmail: u.UserSecuritySettings.RecoveryEmail,
                    CommunicationMethod: u.UserSecuritySettings.DefaultCommunicationMethod, 
                    PreferredLanguage: u.PreferredLanguage, Status: u.Status))
                .FirstOrDefaultAsync(cancellationToken);

        if (fallBackUserModel == null)
            return null;

        await _cacheService.SetAsync(key, fallBackUserModel, _cacheDuration, cancellationToken);

        return fallBackUserModel;
    }

    public async Task<Result> RemoveUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var key = _identityCacheKeyFactory.GetUserKey(userId);
        await _cacheService.RemoveAsync(key, cancellationToken);
        return Result.Success(OperationStatusCode.Success);
    }

    public async Task<Result> SetUserAsync(UserModel userModel, CancellationToken cancellationToken = default)
    {
        var key = _identityCacheKeyFactory.GetUserKey(userModel.UserId);
        await _cacheService.SetAsync(key, userModel, _cacheDuration, cancellationToken);
        return Result.Success(OperationStatusCode.Success);
    }

    public async Task<Result> SetUserActivationChanlageAsync(Guid userId, string token,  CancellationToken cancellationToken = default)
    {
        var key = _identityCacheKeyFactory.GetUserActivationChanlageKey(userId);
        await _cacheService.SetAsync(key, token, _cacheDuration);
        return Result.Success(OperationStatusCode.Success);
    }

    public async Task<Result> VerifyUserActivationChanlageAsync(Guid userId, string token, CancellationToken cancellationToken = default)
    {
        var key = _identityCacheKeyFactory.GetUserActivationChanlageKey(userId);
        var chanlage = await _cacheService.GetAsync<string>(key, cancellationToken);

        if (chanlage == null || chanlage != token)
        {
            return Result.Failure(OperationStatusCode.InvalidInput);
        }

        return Result.Success(OperationStatusCode.Success);
    }

    public async Task<Result> CompleteUserActivationChanlageAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var key = _identityCacheKeyFactory.GetUserActivationChanlageKey(userId);
        await _cacheService.RemoveAsync(key, cancellationToken);
        return Result.Success(OperationStatusCode.Success);
    }
}