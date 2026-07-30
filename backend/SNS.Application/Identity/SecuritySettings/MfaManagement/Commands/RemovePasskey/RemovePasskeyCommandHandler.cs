using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Domain.Identity.SecuritySettings.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace SNS.Application.Identity.SecuritySettings.MfaManagement.Commands.RemovePasskey;

/// <summary>
/// Handles the execution of <see cref="RemovePasskeyCommand"/> to remove an existing passkey for the current user.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Retrieves the current authenticated user ID.
/// 2. Fetches the target passkey from repository and validates ownership.
/// 3. Deletes the passkey entity and commits changes to the database.
/// Side effects include entity deletion from the persistence store.
/// </remarks>
internal sealed class RemovePasskeyCommandHandler : ICommandHandler<RemovePasskeyCommand>
{
    private readonly IRepository<UserPasskey> _passkeyRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public RemovePasskeyCommandHandler(
        IRepository<UserPasskey> passkeyRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _passkeyRepository = passkeyRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task<Result> Handle(
        RemovePasskeyCommand request,
        CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;

        var passkey = await _passkeyRepository.GetByIdAsync(request.PasskeyId, cancellationToken);

        if (passkey is null || passkey.UserId != userId)
        {
            return Result.Failure(UserStatusCodes.NotFound);
        }

        _passkeyRepository.Delete(passkey);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(OperationStatusCode.Success);
    }
}