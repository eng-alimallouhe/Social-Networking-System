using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Jobs;

namespace SNS.Application.Jobs.Companies.Commands.UpdateCompany;

public sealed record UpdateCompanyCommand(
    Guid CompanyId,
    string Name,
    string Industry,
    string? WebsiteUrl = null,
    string? LogoObjectKey = null
) : ICommand;

internal sealed class UpdateCompanyCommandHandler : ICommandHandler<UpdateCompanyCommand>
{
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCompanyCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateCompanyCommand request, CancellationToken cancellationToken)
    {
        var currentProfileId = _currentUserService.ProfileId;
        if (!currentProfileId.HasValue)
        {
            return Result.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var company = await _dbContext.Companies
            .FirstOrDefaultAsync(c => c.Id == request.CompanyId && c.IsActive, cancellationToken);

        if (company == null)
        {
            return Result.Failure(CompanyStatusCodes.CompanyNotFound);
        }

        var isAdministrator = await _dbContext.CompanyAdministrators
            .AnyAsync(ca => ca.CompanyId == request.CompanyId && ca.ProfileId == currentProfileId.Value, cancellationToken);

        if (!isAdministrator)
        {
            return Result.Failure(CompanyStatusCodes.NotCompanyAdmin);
        }

        company.Update(
            name: request.Name,
            industry: request.Industry,
            websiteUrl: request.WebsiteUrl,
            logoObjectKey: request.LogoObjectKey);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(CompanyStatusCodes.CompanyUpdated);
    }
}
