namespace SNS.Application.Resumes.Certificates.Contracts;

/// <summary>
/// Represents professional certification details within a resume.
/// </summary>
/// <param name="Id">The unique identifier of the certificate record.</param>
/// <param name="ResumeId">The identifier of the parent resume.</param>
/// <param name="Title">The title or name of the certification.</param>
/// <param name="Issuer">The issuing authority or institution.</param>
/// <param name="IssueDate">The date when the certificate was issued.</param>
public sealed record ResumeCertificateDto(
    Guid Id,
    Guid ResumeId,
    string Title,
    string Issuer,
    DateTime IssueDate
);
