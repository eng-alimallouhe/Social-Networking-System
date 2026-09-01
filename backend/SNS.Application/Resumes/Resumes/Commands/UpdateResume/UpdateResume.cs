using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Resumes.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Domain.Shared.Enums;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Resumes.Commands.UpdateResume;

/// <summary>
/// Represents a command to update an existing resume's core metadata, template, summary, and language settings.
/// </summary>
/// <param name="ResumeId">The unique identifier of the resume to update.</param>
/// <param name="PersonalPictureUrl">The optional personal picture storage object key if not syncing with profile.</param>
/// <param name="SyncProfilePicture">Whether the resume synchronizes its picture with the profile avatar.</param>
/// <param name="Title">The updated title or designation.</param>
/// <param name="Template">The updated visual layout template.</param>
/// <param name="Summary">The updated professional summary.</param>
/// <param name="Language">The updated supported language.</param>
public sealed record UpdateResumeCommand(
    Guid ResumeId,
    string? PersonalPictureUrl,
    bool SyncProfilePicture,
    string Title,
    Template Template,
    string Summary,
    SupportedLanguage Language
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="UpdateResumeCommand"/> to modify an existing resume.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Fetches the target resume and verifies ownership.
/// 3. Validates incoming parameters.
/// 4. Invokes the update method on the domain entity.
/// 5. Commits changes via unit of work.
/// Side effects include entity property updates and database persistence.
/// </remarks>
internal sealed class UpdateResumeCommandHandler : ICommandHandler<UpdateResumeCommand>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateResumeCommandHandler(
        ISoftDeletableRepository<Resume> resumeRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _resumeRepo = resumeRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateResumeCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return Result.Failure(OperationStatusCode.InvalidInput);
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

        resume.Update(
            request.PersonalPictureUrl,
            request.SyncProfilePicture,
            request.Title,
            request.Template,
            request.Summary ?? string.Empty,
            request.Language
        );

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ResumeStatusCodes.ResumeUpdated);
    }
}
