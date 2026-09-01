using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Certificates.Commands.DeleteResumeCertificate;

/// <summary>
/// Represents a command to remove a certificate entry from a resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the parent resume.</param>
/// <param name="CertificateId">The unique identifier of the certificate record to delete.</param>
public sealed record DeleteResumeCertificateCommand(Guid ResumeId, Guid CertificateId) : ICommand;

/// <summary>
/// Handles the execution of <see cref="DeleteResumeCertificateCommand"/> to delete a certificate record.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies parent resume existence and ownership.
/// 3. Validates certificate entry existence and association.
/// 4. Removes the entity from the repository.
/// 5. Commits changes via unit of work.
/// Side effects include hard deletion and database commit.
/// </remarks>
internal sealed class DeleteResumeCertificateCommandHandler : ICommandHandler<DeleteResumeCertificateCommand>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeCertificate> _certificateRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteResumeCertificateCommandHandler(
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

    public async Task<Result> Handle(DeleteResumeCertificateCommand request, CancellationToken cancellationToken)
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

        var certificate = await _certificateRepo.GetByIdAsync(request.CertificateId, cancellationToken);
        if (certificate == null || certificate.ResumeId != request.ResumeId)
        {
            return Result.Failure(ResumeStatusCodes.CertificateNotFound);
        }

        _certificateRepo.Delete(certificate);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(ResumeStatusCodes.CertificateDeleted);
    }
}
