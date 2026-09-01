using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Resumes.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Languages.Commands.UpdateResumeLanguage;

/// <summary>
/// Represents a command to update an existing language entry on a resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the parent resume.</param>
/// <param name="LanguageId">The unique identifier of the language record to update.</param>
/// <param name="Language">The updated language.</param>
/// <param name="Level">The updated proficiency level.</param>
public sealed record UpdateResumeLanguageCommand(
    Guid ResumeId,
    Guid LanguageId,
    Language Language,
    LanguageLevel Level
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="UpdateResumeLanguageCommand"/> to update a language record.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies parent resume existence and ownership.
/// 3. Validates language entry existence and association with the resume.
/// 4. Updates entity properties via domain method.
/// 5. Commits changes via unit of work.
/// Side effects include entity property updates and database commit.
/// </remarks>
internal sealed class UpdateResumeLanguageCommandHandler : ICommandHandler<UpdateResumeLanguageCommand>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeLanguage> _languageRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateResumeLanguageCommandHandler(
        ISoftDeletableRepository<Resume> resumeRepo,
        IRepository<ResumeLanguage> languageRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _resumeRepo = resumeRepo;
        _languageRepo = languageRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateResumeLanguageCommand request, CancellationToken cancellationToken)
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

        var language = await _languageRepo.GetByIdAsync(request.LanguageId, cancellationToken);
        if (language == null || language.ResumeId != request.ResumeId)
        {
            return Result.Failure(ResumeStatusCodes.LanguageNotFound);
        }

        language.Update(
            request.Language,
            request.Level
        );

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ResumeStatusCodes.LanguageUpdated);
    }
}
