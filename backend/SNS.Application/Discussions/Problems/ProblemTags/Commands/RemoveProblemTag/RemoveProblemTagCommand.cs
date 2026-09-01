using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Discussions.Problems.Relations;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Problems.ProblemTags.Commands.RemoveProblemTag;

/// <summary>
/// Command to remove an existing tag association from a discussion problem.
/// </summary>
/// <param name="ProblemId">The unique identifier of the problem.</param>
/// <param name="TagId">The unique identifier of the tag to disassociate.</param>
public sealed record RemoveProblemTagCommand(
    Guid ProblemId,
    Guid TagId
) : ICommand;

/// <summary>
/// Handles <see cref="RemoveProblemTagCommand"/> to verify problem ownership and remove the tag link.
/// </summary>
internal sealed class RemoveProblemTagCommandHandler : ICommandHandler<RemoveProblemTagCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<ProblemTag> _problemTagRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveProblemTagCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<ProblemTag> problemTagRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _problemTagRepo = problemTagRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveProblemTagCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var problem = await _dbContext.Problems
            .FirstOrDefaultAsync(p => p.Id == request.ProblemId && p.IsActive, cancellationToken);

        if (problem == null)
        {
            return Result.Failure(ProblemStatusCodes.ProblemNotFound);
        }

        if (problem.AuthorId != profileId.Value)
        {
            return Result.Failure(ProblemStatusCodes.NotProblemOwner);
        }

        var problemTag = await _problemTagRepo.GetSingleByExpressionAsync(
            pt => pt.ProblemId == request.ProblemId && pt.TagId == request.TagId,
            cancellationToken);

        if (problemTag == null)
        {
            return Result.Failure(ProblemStatusCodes.TagNotFound);
        }

        _problemTagRepo.Delete(problemTag);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProblemStatusCodes.TagRemoved);
    }
}
