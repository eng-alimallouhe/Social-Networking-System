using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Languages.Commands.DeleteResumeLanguage;

/// <summary>
/// Represents a command to remove a language proficiency entry from a resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the parent resume.</param>
/// <param name="LanguageId">The unique identifier of the language record to delete.</param>
public sealed record DeleteResumeLanguageCommand(Guid ResumeId, Guid LanguageId) : ICommand;

/// <summary>
/// Handles the execution of <see cref="DeleteResumeLanguageCommand"/> to delete a language record.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies parent resume existence and ownership.
/// 3. Validates language entry existence and association.
/// 4. Removes the entity from the repository.
/// 5. Commits changes via unit of work.
/// Side effects include hard deletion and database commit.
/// </remarks>
internal sealed class DeleteResumeLanguageCommandHandler : ICommandHandler<DeleteResumeLanguageCommand>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeLanguage> _languageRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteResumeLanguageCommandHandler(
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

    public async Task<Result> Handle(DeleteResumeLanguageCommand request, CancellationToken cancellationToken)
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

        _languageRepo.Delete(language);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ResumeStatusCodes.LanguageDeleted);
    }
}
