using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Discussions.Problems.Relations;
using SNS.Domain.Preferences.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Discussions;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Discussions.Problems.ProblemTags.Commands.AddProblemTag;

/// <summary>
/// Command to attach a tag to an existing discussion problem.
/// </summary>
/// <param name="ProblemId">The unique identifier of the problem.</param>
/// <param name="TagName">The name of the tag to add.</param>
public sealed record AddProblemTagCommand(
    Guid ProblemId,
    string TagName
) : ICommand;

/// <summary>
/// Handles <see cref="AddProblemTagCommand"/> to verify problem ownership, match/create tag, and establish the relation.
/// </summary>
internal sealed class AddProblemTagCommandHandler : ICommandHandler<AddProblemTagCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly IRepository<ProblemTag> _problemTagRepo;
    private readonly IRepository<Tag> _tagRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddProblemTagCommandHandler(
        IApplicationDbContext dbContext,
        IRepository<ProblemTag> problemTagRepo,
        IRepository<Tag> tagRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _problemTagRepo = problemTagRepo;
        _tagRepo = tagRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(AddProblemTagCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (!profileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        if (string.IsNullOrWhiteSpace(request.TagName))
        {
            return Result.Failure(OperationStatusCode.InvalidInput);
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

        var normalizedName = request.TagName.Trim().ToLower();

        var tag = await _dbContext.Tags
            .FirstOrDefaultAsync(t => t.Name.ToLower() == normalizedName, cancellationToken);

        if (tag == null)
        {
            tag = Tag.Create(normalizedName);
            _tagRepo.Add(tag);
            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        var alreadyAssociated = await _dbContext.ProblemTags
            .AnyAsync(pt => pt.ProblemId == request.ProblemId && pt.TagId == tag.Id, cancellationToken);

        if (alreadyAssociated)
        {
            return Result.Failure(ProblemStatusCodes.TagAlreadyExists);
        }

        var problemTag = ProblemTag.Create(request.ProblemId, tag.Id);
        _problemTagRepo.Add(problemTag);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ProblemStatusCodes.TagAdded);
    }
}
