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

namespace SNS.Application.Jobs.CompanyCreateRequests.Commands.ApproveCompanyCreateRequest;

public sealed record ApproveCompanyCreateRequestCommand(
    Guid RequestId,
    string? ReviewNote = null
) : ICommand<Guid>;

internal sealed class ApproveCompanyCreateRequestCommandHandler : ICommandHandler<ApproveCompanyCreateRequestCommand, Guid>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly ISoftDeletableRepository<Company> _companyRepository;
    private readonly IRepository<CompanyAdministrator> _adminRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveCompanyCreateRequestCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        ISoftDeletableRepository<Company> companyRepository,
        IRepository<CompanyAdministrator> adminRepository,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _companyRepository = companyRepository;
        _adminRepository = adminRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ApproveCompanyCreateRequestCommand request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var entity = await _dbContext.CompanyCreateRequests
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (entity == null)
        {
            return Result<Guid>.Failure(CompanyCreateRequestStatusCodes.RequestNotFound);
        }

        if (entity.Status != CompanyCreateRequestStatus.Pending)
        {
            return Result<Guid>.Failure(CompanyCreateRequestStatusCodes.RequestNotPending);
        }

        // Create the company
        var company = Company.Create(
            name: entity.Name,
            industry: entity.Industry,
            websiteUrl: entity.WebsiteUrl,
            logoObjectKey: entity.LogoObjectKey);

        _companyRepository.Add(company);

        // Assign submitter as Owner
        var admin = CompanyAdministrator.Create(
            companyId: company.Id,
            profileId: entity.ProfileId,
            adminRole: CompanyRole.Owner);

        _adminRepository.Add(admin);

        // Approve request
        entity.Approve(
            createdCompanyId: company.Id,
            reviewedByProfileId: currentProfileId.Value,
            reviewNote: request.ReviewNote);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(company.Id, CompanyCreateRequestStatusCodes.RequestApproved);
    }
}
