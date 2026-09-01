using SNS.Application.Abstractions.Messaging;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Shared.Abstractions.Data;
using SNS.Domain.Resumes.Entities;
using SNS.Domain.Shared.Abstractions.Repositories;
using SNS.Shared.Results;
using SNS.Shared.StatusCodes;
using SNS.Shared.StatusCodes.Identity;
using SNS.Shared.StatusCodes.Resumes;

namespace SNS.Application.Resumes.Certificates.Commands.AddResumeCertificate;

/// <summary>
/// Represents a command to add a professional certification entry to a resume.
/// </summary>
/// <param name="ResumeId">The unique identifier of the target resume.</param>
/// <param name="Title">The title of the certification.</param>
/// <param name="Issuer">The issuing organization or body.</param>
/// <param name="IssueDate">The date the certificate was issued.</param>
public sealed record AddResumeCertificateCommand(
    Guid ResumeId,
    string Title,
    string Issuer,
    DateTime IssueDate
) : ICommand<Guid>;

/// <summary>
/// Handles the execution of <see cref="AddResumeCertificateCommand"/> to attach a certificate record.
/// </summary>
/// <remarks>
/// Business operation and processing flow:
/// 1. Resolves authenticated user profile ID.
/// 2. Verifies parent resume existence and ownership.
/// 3. Validates required title and issuer parameters.
/// 4. Instantiates <see cref="ResumeCertificate"/> and persists it via repository.
/// 5. Commits changes via unit of work.
/// Side effects include database insert and transaction commit.
/// </remarks>
internal sealed class AddResumeCertificateCommandHandler : ICommandHandler<AddResumeCertificateCommand, Guid>
{
    private readonly ISoftDeletableRepository<Resume> _resumeRepo;
    private readonly IRepository<ResumeCertificate> _certificateRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly IUnitOfWork _unitOfWork;

    public AddResumeCertificateCommandHandler(
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

    public async Task<Result<Guid>> Handle(AddResumeCertificateCommand request, CancellationToken cancellationToken)
    {
        var profileId = _currentUserService.ProfileId;
        if (profileId == null)
        {
            return Result<Guid>.Failure(SecurityStatusCodes.AuthenticationRequired);
        }

        var resume = await _resumeRepo.GetByIdAsync(request.ResumeId, cancellationToken);
        if (resume == null || !resume.IsActive)
        {
            return Result<Guid>.Failure(ResumeStatusCodes.ResumeNotFound);
        }

        if (resume.OwnerId != profileId.Value)
        {
            return Result<Guid>.Failure(ResumeStatusCodes.NotResumeOwner);
        }

        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Issuer))
        {
            return Result<Guid>.Failure(OperationStatusCode.InvalidInput);
        }

        var certificate = ResumeCertificate.Create(
            resumeId: request.ResumeId,
            title: request.Title,
            issuer: request.Issuer,
            issueDate: request.IssueDate
        );

        _certificateRepo.Add(certificate);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result<Guid>.Success(certificate.Id, ResumeStatusCodes.CertificateAdded);
    }
}
