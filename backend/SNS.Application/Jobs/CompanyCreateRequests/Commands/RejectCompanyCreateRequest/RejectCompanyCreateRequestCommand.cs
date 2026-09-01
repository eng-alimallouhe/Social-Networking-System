using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Jobs.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.CompanyCreateRequests.Commands.RejectCompanyCreateRequest;

public sealed record RejectCompanyCreateRequestCommand(
    Guid RequestId,
    string? ReviewNote = null
) : ICommand;

internal sealed class RejectCompanyCreateRequestCommandHandler : ICommandHandler<RejectCompanyCreateRequestCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RejectCompanyCreateRequestCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RejectCompanyCreateRequestCommand request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var entity = await _dbContext.CompanyCreateRequests
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null)
        {
            return Result.Failure(CompanyCreateRequestStatusCodes.RequestNotFound);
        }

        if (entity.Status != CompanyCreateRequestStatus.Pending)
        {
            return Result.Failure(CompanyCreateRequestStatusCodes.RequestNotPending);
        }

        entity.Reject(
            reviewedByProfileId: currentProfileId.Value,
            reviewNote: request.ReviewNote);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(CompanyCreateRequestStatusCodes.RequestRejected);
    }
}
