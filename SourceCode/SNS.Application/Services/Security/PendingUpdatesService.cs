using SNS.Application.Abstractions.Security;
using SNS.Common.Results;
using SNS.Common.StatusCodes.Common;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Security.Entities;
using SNS.Domain.Security.Enums;

namespace SNS.Application.Services.Security;

public class PendingUpdatesService : IPendingUpdatesService
{
    private readonly IRepository<PendingUpdate> _pendingRepo;

    public PendingUpdatesService(IRepository<PendingUpdate> pendingRepo)
    {
        _pendingRepo = pendingRepo;
    }

    public async Task<Result<Guid>> CreateOrReplaceAsync(
        Guid userId, 
        UpdateType type, 
        string info, 
        Guid? supportId = null)
    {
        // 1. Check for existing pending update of the same type for this user
        // We assume we want to enforce: "One pending update of Type X per User"
        var existingUpdate = await _pendingRepo.GetSingleByExpressionAsync(
            p => p.UserId == userId && p.UpdateType == type);

        // 2. Delete existing if found (Hard Delete)
        if (existingUpdate != null)
        {
            await _pendingRepo.DeleteAsync(existingUpdate.Id);
        }

        // 3. Create new update
        var newUpdate = new PendingUpdate
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UpdateType = type,
            UpdatedInfo = info,
            RequestedAt = DateTime.UtcNow,
            SupportId = supportId
        };

        // 4. Add to repository
        await _pendingRepo.AddAsync(newUpdate);

        return Result<Guid>.Success(newUpdate.Id, ResourceStatusCode.Found);
    }

    public async Task<Result<PendingUpdate>> GetPendingByInfoAsync(string info, UpdateType type)
    {
        var update = await _pendingRepo.GetSingleByExpressionAsync(
            p => p.UpdatedInfo == info && p.UpdateType == type);

        if (update == null)
        {
            return Result<PendingUpdate>.Failure(ResourceStatusCode.NotFound);
        }

        return Result<PendingUpdate>.Success(update, ResourceStatusCode.Found);
    }

    public async Task<Result<PendingUpdate>> GetPendingByUserAsync(Guid userId, UpdateType type)
    {
        var update = await _pendingRepo.GetSingleByExpressionAsync(
            p => p.UserId == userId && p.UpdateType == type);

        if (update == null)
        {
            return Result<PendingUpdate>.Failure(ResourceStatusCode.NotFound);
        }

        return Result<PendingUpdate>.Success(update, ResourceStatusCode.Found);
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        await _pendingRepo.DeleteAsync(id);
        return Result.Success(ResourceStatusCode.Found);
    }
}