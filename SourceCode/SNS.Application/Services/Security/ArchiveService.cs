using SNS.Application.Abstractions.Security;
using SNS.Common.Results;
using SNS.Common.StatusCodes.Common;
using SNS.Domain.Abstractions.Repositories;
using SNS.Domain.Security.Entities;
using SNS.Domain.Security.Enums;

namespace SNS.Application.Services.Security;

public class ArchiveService : IArchiveService
{
    private readonly IRepository<UserArchive> _userArchiveRepo;
    private readonly IRepository<IdentityArchive> _identityArchiveRepo;
    private readonly IRepository<PasswordArchive> _passwordArchiveRepo;

    public ArchiveService(
        IRepository<UserArchive> userArchiveRepo,
        IRepository<IdentityArchive> identityArchiveRepo,
        IRepository<PasswordArchive> passwordArchiveRepo)
    {
        _userArchiveRepo = userArchiveRepo;
        _identityArchiveRepo = identityArchiveRepo;
        _passwordArchiveRepo = passwordArchiveRepo;
    }

    public async Task<Result> LogUserActionAsync(Guid userId, ActionType actionType, Guid performedBy, string? reason = null)
    {
        var archiveEntry = new UserArchive
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ActionType = actionType,
            PerformedBy = performedBy,
            Reason = reason,
            CreatedAt = DateTime.UtcNow,
            TimeStamp = DateTime.UtcNow
        };

        await _userArchiveRepo.AddAsync(archiveEntry);

        return Result.Success(Resources.ResourceFound);
    }

    public async Task<Result> ArchiveIdentityAsync(Guid userId, string identifier, IdentityType identityType)
    {
        var identityEntry = new IdentityArchive
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            UserIdentifier = identifier,
            IdentityType = identityType,
            ChangedAt = DateTime.UtcNow
        };

        await _identityArchiveRepo.AddAsync(identityEntry);

        return Result.Success(Resources.ResourceFound);
    }

    public async Task<Result> ArchivePasswordAsync(Guid userId, string hashedPassword)
    {
        var passwordEntry = new PasswordArchive
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            HashedPassword = hashedPassword,
            ChangedAt = DateTime.UtcNow
        };

        await _passwordArchiveRepo.AddAsync(passwordEntry);

        return Result.Success(Resources.ResourceFound);
    }
}