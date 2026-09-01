using MediatR;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Application.Shared.Events;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Resumes.Events;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Resumes.Commands.DeleteResume;

/// <summary>
/// Represents a command to soft-delete an existing resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the resume to delete.</param>
public sealed record DeleteResumeCommand(Guid ResumeId) : ICommand;

/// <summary>
/// Handles the execution of <see cref="DeleteResumeCommand"/> to perform soft deletion on a resume.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Fetches target resume and verifies ownership.
/// 3. Marks the entity as inactive using soft deletion.
/// 4. Commits changes via unit of work.
/// 5. Publishes <see cref="ResumeDeletedIntegrationEvent"/> to reverse reputation points.
/// Side effects include soft deletion flag update, reputation penalty ledger insert, and database commit.
/// </remarks>
internal sealed class DeleteResumeCommandHandler : ICommandHandler<DeleteResumeCommand>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;

    public DeleteResumeCommandHandler(
        ISoftDeletableRepository<Resume> resumeRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _resumeRepo = resumeRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
    }

    public async Task<Result> Handle(DeleteResumeCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var resume = await _resumeRepo.GetByIdAsync(request.ResumeId, cancellationToken);
        if (resume == null || !resume.IsActive)
        {
            return Result.Failure(ResumeStatusCodes.ResumeNotFound);
        }

        if (resume.OwnerId != profileId.Value)
        {
            return Result.Failure(ResumeStatusCodes.NotResumeOwner);
        }

        resume.SoftDelete();
        await _unitOfWork.CompleteAsync(cancellationToken);

        await _mediator.Publish(
            new DomainEventNotification<ResumeDeletedIntegrationEvent>(
                new ResumeDeletedIntegrationEvent(resume.OwnerId, resume.Id, DateTime.UtcNow)),
            cancellationToken);

        return Result.Success(ResumeStatusCodes.ResumeDeleted);
    }
}
