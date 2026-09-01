using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Projects.Commands.RemoveResumeProject;

/// <summary>
/// Represents a command to disassociate a project from a resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the parent resume.</param>
/// <param name="ProjectId">The unique identifier of the linked project to remove.</param>
public sealed record RemoveResumeProjectCommand(Guid ResumeId, Guid ProjectId) : ICommand;

/// <summary>
/// Handles the execution of <see cref="RemoveResumeProjectCommand"/> to unlink a project from a resume.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies parent resume existence and ownership.
/// 3. Validates that the bridge relationship exists.
/// 4. Removes the bridge link via repository.
/// 5. Commits changes via unit of work.
/// Side effects include bridge record deletion and database commit.
/// </remarks>
internal sealed class RemoveResumeProjectCommandHandler : ICommandHandler<RemoveResumeProjectCommand>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeProject> _resumeProjectRepo;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveResumeProjectCommandHandler(
        ISoftDeletableRepository<Resume> resumeRepo,
        IRepository<ResumeProject> resumeProjectRepo,
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _resumeRepo = resumeRepo;
        _resumeProjectRepo = resumeProjectRepo;
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveResumeProjectCommand request, CancellationToken cancellationToken)
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

        var resumeProject = await _dbContext.ResumeProjects
            .FirstOrDefaultAsync(rp => rp.ResumeId == request.ResumeId && rp.ProjectId == request.ProjectId, cancellationToken);

        if (resumeProject == null)
        {
            return Result.Failure(ResumeStatusCodes.ResumeProjectNotFound);
        }

        _resumeProjectRepo.Delete(resumeProject);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ResumeStatusCodes.ProjectRemoved);
    }
}
