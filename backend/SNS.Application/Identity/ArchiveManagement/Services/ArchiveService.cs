using SNS.Application.Identity.ArchiveManagement.Abstractions;
using SNS.Application.Identity.Shared.DTOs.Archives;
using SNS.Shared.Results;
using SNS.Domain.Identity.ArchiveManagement.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.StatusCodes;

namespace SNS.Application.Identity.ArchiveManagement.Services;

public class ArchiveService : IArchiveService
{
    private readonly IRepository<UserArchive> _userArchiveRepo;
    private readonly IRepository<IdentityArchive> _identityArchiveRepo;
    private readonly IRepository<PasswordArchive> _passwordArchiveRepo;

    public ArchiveService(
        IRepository<UserArchive> userArchiveRepo,
        IRepository<IdentityArchive> identityRepo,
        IRepository<PasswordArchive> passwordRepo)
    {
        _userArchiveRepo = userArchiveRepo;
        _identityArchiveRepo = identityRepo;
        _passwordArchiveRepo = passwordRepo;
    }

    public async Task<Result> ArchiveIdentityAsync(CreateIdentityArchiveDto dto, CancellationToken cancellationToken = default)
    {
        var archive = IdentityArchive.Create(
            userId: dto.UserId,
            oldUserIdentifier: dto.OldIdentifier,
            newUserIdentifier: dto.NewIdentifier,
            type: dto.IdentityType
        );

        _identityArchiveRepo.Add(archive);

        return Result.Success(OperationStatusCode.Success);
    }

    public async Task<Result> ArchivePasswordAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var archive = PasswordArchive.Create(
            userId: userId
        );

        _passwordArchiveRepo.Add(archive);

        return Result.Success(OperationStatusCode.Success);
    }

    public async Task<Result> LogUserActionAsync(CreateUserArchiveDto dto, CancellationToken cancellationToken = default)
    {
        var archive = UserArchive.Create(
            targetId: dto.UserId,
            performedById: dto.PerformedBy,
            type: dto.ActionType,
            reason: dto.Reason,
            parameters: dto.Parameters
        );

        _userArchiveRepo.Add(archive);

        return Result.Success(OperationStatusCode.Success);
    }
}
