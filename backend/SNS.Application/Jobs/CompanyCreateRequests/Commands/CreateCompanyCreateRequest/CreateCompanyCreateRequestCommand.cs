using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Jobs.Entities;
using SNS.Domain.Jobs.Enums;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.CompanyCreateRequests.Commands.CreateCompanyCreateRequest;

public sealed record CreateCompanyCreateRequestCommand(
    string Name,
    string Industry,
    string? WebsiteUrl = null,
    string? LogoObjectKey = null
) : ICommand<Guid>;

internal sealed class CreateCompanyCreateRequestCommandHandler : ICommandHandler<CreateCompanyCreateRequestCommand, Guid>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<CompanyCreateRequest> _requestRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateCompanyCreateRequestCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IRepository<CompanyCreateRequest> requestRepository,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _requestRepository = requestRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateCompanyCreateRequestCommand request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var hasDuplicate = await _dbContext.CompanyCreateRequests
            .AnyAsync(r => r.ProfileId == currentProfileId.Value &&
                           r.Name.ToLower() == request.Name.Trim().ToLower() &&
                           r.Status == CompanyCreateRequestStatus.Pending, cancellationToken);

        if (hasDuplicate)
        {
            return Result<Guid>.Failure(CompanyCreateRequestStatusCodes.DuplicatePendingRequest);
        }

        var entity = CompanyCreateRequest.Create(
            profileId: currentProfileId.Value,
            name: request.Name.Trim(),
            industry: request.Industry.Trim(),
            websiteUrl: request.WebsiteUrl?.Trim(),
            logoObjectKey: request.LogoObjectKey?.Trim());

        _requestRepository.Add(entity);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(entity.Id, CompanyCreateRequestStatusCodes.RequestCreated);
    }
}
