using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Projects.Commands.AddResumeProject;

/// <summary>
/// Represents a command to associate an existing project with a resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the target resume.</param>
/// <param name="ProjectId">The unique identifier of the project to link.</param>
public sealed record AddResumeProjectCommand(Guid ResumeId, Guid ProjectId) : ICommand;

/// <summary>
/// Handles the execution of <see cref="AddResumeProjectCommand"/> to link a project to a resume.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies parent resume existence and ownership.
/// 3. Validates that the target project exists and is active.
/// 4. Checks for existing bridge link to avoid duplicates.
/// 5. Instantiates <see cref="ResumeProject"/> bridge and persists via repository.
/// 6. Commits changes via unit of work.
/// Side effects include database insert and transaction commit.
/// </remarks>
internal sealed class AddResumeProjectCommandHandler : ICommandHandler<AddResumeProjectCommand>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeProject> _resumeProjectRepo;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddResumeProjectCommandHandler(
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

    public async Task<Result> Handle(AddResumeProjectCommand request, CancellationToken cancellationToken)
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

        var projectExists = await _dbContext.Projects
            .AnyAsync(p => p.Id == request.ProjectId && p.IsActive, cancellationToken);

        if (!projectExists)
        {
            return Result.Failure(ResumeStatusCodes.ProjectNotFound);
        }

        var alreadyLinked = await _dbContext.ResumeProjects
            .AnyAsync(rp => rp.ResumeId == request.ResumeId && rp.ProjectId == request.ProjectId, cancellationToken);

        if (alreadyLinked)
        {
            return Result.Failure(ResumeStatusCodes.ProjectAlreadyAdded);
        }

        var resumeProject = ResumeProject.Create(request.ResumeId, request.ProjectId);
        _resumeProjectRepo.Add(resumeProject);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ResumeStatusCodes.ProjectAdded);
    }
}
