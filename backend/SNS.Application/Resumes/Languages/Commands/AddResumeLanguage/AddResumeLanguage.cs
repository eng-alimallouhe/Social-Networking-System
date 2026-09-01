using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Resumes.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Languages.Commands.AddResumeLanguage;

/// <summary>
/// Represents a command to add a language proficiency entry to a resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the target resume.</param>
/// <param name="Language">The language.</param>
/// <param name="Level">The proficiency level.</param>
public sealed record AddResumeLanguageCommand(
    Guid ResumeId,
    Language Language,
    LanguageLevel Level
) : ICommand<Guid>;

/// <summary>
/// Handles the execution of <see cref="AddResumeLanguageCommand"/> to attach a language record.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies parent resume existence and ownership.
/// 3. Instantiates <see cref="ResumeLanguage"/> and persists it via repository.
/// 4. Commits changes via unit of work.
/// Side effects include database insert and transaction commit.
/// </remarks>
internal sealed class AddResumeLanguageCommandHandler : ICommandHandler<AddResumeLanguageCommand, Guid>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeLanguage> _languageRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddResumeLanguageCommandHandler(
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

    public async Task<Result<Guid>> Handle(AddResumeLanguageCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var resume = await _resumeRepo.GetByIdAsync(request.ResumeId, cancellationToken);
        if (resume == null || !resume.IsActive)
        {
            return Result<Guid>.Failure(ResumeStatusCodes.ResumeNotFound);
        }

        if (resume.OwnerId != profileId.Value)
        {
            return Result<Guid>.Failure(ResumeStatusCodes.NotResumeOwner);
        }

        var language = ResumeLanguage.Create(
            resumeId: request.ResumeId,
            language: request.Language,
            level: request.Level
        );

        _languageRepo.Add(language);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(language.Id, ResumeStatusCodes.LanguageAdded);
    }
}
