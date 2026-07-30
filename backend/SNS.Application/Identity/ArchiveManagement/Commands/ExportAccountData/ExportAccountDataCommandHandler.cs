using Microsoft.EntityFrameworkCore;
using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.ArchiveManagement.Abstractions;
using SNS.Application.Identity.ArchiveManagement.Contracts;
using SNS.Application.Identity.ArchiveManagement.Services;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.BackgroundJobs;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Identity.ArchiveManagement.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;

namespace SNS.Application.Identity.ArchiveManagement.Commands.ExportAccountData;

public sealed class ExportAccountDataCommandHandler
    : ICommandHandler<ExportAccountDataCommand, ExportAccountDataResponseDto>
{
    private readonly IRepository<ExportDataRequest> _exportDataRequestRepo;
    private readonly IApplicationDbContext _dbContext;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJobSchedulerService _jobSchedulerService;

    public ExportAccountDataCommandHandler(
        IApplicationDbContext dbContext,
        ICurrentUserService currentUserService,
        IRepository<ExportDataRequest> exportDataRequestRepo,
        IUnitOfWork unitOfWork,
        IJobSchedulerService jobSchedulerService)
    {
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _exportDataRequestRepo = exportDataRequestRepo;
        _unitOfWork = unitOfWork;
        _jobSchedulerService = jobSchedulerService;
    }

    public async Task<Result<ExportAccountDataResponseDto>> Handle(
        ExportAccountDataCommand request,
        CancellationToken cancellationToken)
    {
        var currentUserId = _currentUserService.UserId;

        if (currentUserId == null || currentUserId == Guid.Empty)
        {
            return Result<ExportAccountDataResponseDto>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var hasActiveRequest = await _dbContext.ExportDataRequests
            .AnyAsync(r => r.UserId == currentUserId &&
                          (r.Status == ExportStatus.Pending || r.Status == ExportStatus.Processing),
                          cancellationToken);

        if (hasActiveRequest)
        {
            return Result<ExportAccountDataResponseDto>.Failure(OperationStatusCode.Conflict);
        }

        var exportRequest = ExportDataRequest.Create(currentUserId.Value);

        _exportDataRequestRepo.Add(exportRequest);
        await _unitOfWork.CompleteAsync(cancellationToken);

        _jobSchedulerService.Enqueue<IExportDataWorker>(t => t.ProcessExportAsync(exportRequest.Id));

        var response = new ExportAccountDataResponseDto(
            exportRequest.Id,
            exportRequest.Status.ToString(),
            exportRequest.CreatedAt);

        return Result<ExportAccountDataResponseDto>.Success(response, OperationStatusCode.Success);
    }
}