using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Certificates.Commands.UpdateResumeCertificate;

/// <summary>
/// Represents a command to update an existing certificate entry on a resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the parent resume.</param>
/// <param name="CertificateId">The unique identifier of the certificate record to update.</param>
/// <param name="Title">The updated certificate title.</param>
/// <param name="Issuer">The updated issuing authority.</param>
/// <param name="IssueDate">The updated issue date.</param>
public sealed record UpdateResumeCertificateCommand(
    Guid ResumeId,
    Guid CertificateId,
    string Title,
    string Issuer,
    DateTime IssueDate
) : ICommand;

/// <summary>
/// Handles the execution of <see cref="UpdateResumeCertificateCommand"/> to update a certificate record.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies parent resume existence and ownership.
/// 3. Validates certificate entry existence and association with the resume.
/// 4. Updates entity properties via domain method.
/// 5. Commits changes via unit of work.
/// Side effects include entity property updates and database commit.
/// </remarks>
internal sealed class UpdateResumeCertificateCommandHandler : ICommandHandler<UpdateResumeCertificateCommand>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeCertificate> _certificateRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateResumeCertificateCommandHandler(
        ISoftDeletableRepository<Resume> resumeRepo,
        IRepository<ResumeCertificate> certificateRepo,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork)
    {
        _resumeRepo = resumeRepo;
        _certificateRepo = certificateRepo;
        _currentUserService = currentUserService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateResumeCertificateCommand request, CancellationToken cancellationToken)
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

        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Issuer))
        {
            return Result.Failure(OperationStatusCode.InvalidInput);
        }

        var certificate = await _certificateRepo.GetByIdAsync(request.CertificateId, cancellationToken);
        if (certificate == null || certificate.ResumeId != request.ResumeId)
        {
            return Result.Failure(ResumeStatusCodes.CertificateNotFound);
        }

        certificate.Update(
            request.Title,
            request.Issuer,
            request.IssueDate
        );

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ResumeStatusCodes.CertificateUpdated);
    }
}
