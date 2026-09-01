namespace SNS.Shared.StatusCodes.Resumes;

/// <summary>
/// Defines status codes related to the Resume aggregate and its child entities.
/// </summary>
public static class ResumeStatusCodes
{
    private const string Category = "Resumes";

    // Success - 200 / 201
    public static readonly StatusCode ResumeCreated = new(Category, 201);
    public static readonly StatusCode ResumeUpdated = new(Category, 200);
    public static readonly StatusCode ResumeDeleted = new(Category, 200);

    public static readonly StatusCode EducationAdded = new(Category, 201);
    public static readonly StatusCode EducationUpdated = new(Category, 200);
    public static readonly StatusCode EducationDeleted = new(Category, 200);

    public static readonly StatusCode ExperienceAdded = new(Category, 201);
    public static readonly StatusCode ExperienceUpdated = new(Category, 200);
    public static readonly StatusCode ExperienceDeleted = new(Category, 200);

    public static readonly StatusCode CertificateAdded = new(Category, 201);
    public static readonly StatusCode CertificateUpdated = new(Category, 200);
    public static readonly StatusCode CertificateDeleted = new(Category, 200);

    public static readonly StatusCode LanguageAdded = new(Category, 201);
    public static readonly StatusCode LanguageUpdated = new(Category, 200);
    public static readonly StatusCode LanguageDeleted = new(Category, 200);

    public static readonly StatusCode SkillAdded = new(Category, 201);
    public static readonly StatusCode SkillUpdated = new(Category, 200);
    public static readonly StatusCode SkillDeleted = new(Category, 200);

    public static readonly StatusCode ProjectAdded = new(Category, 201);
    public static readonly StatusCode ProjectRemoved = new(Category, 200);

    // Errors - 400 / 403 / 404 / 409
    public static readonly StatusCode ResumeNotFound = new(Category, 404);
    public static readonly StatusCode NotResumeOwner = new(Category, 403);
    public static readonly StatusCode EducationNotFound = new(Category, 404);
    public static readonly StatusCode ExperienceNotFound = new(Category, 404);
    public static readonly StatusCode CertificateNotFound = new(Category, 404);
    public static readonly StatusCode LanguageNotFound = new(Category, 404);
    public static readonly StatusCode SkillNotFound = new(Category, 404);
    public static readonly StatusCode ResumeProjectNotFound = new(Category, 404);
    public static readonly StatusCode ProjectAlreadyAdded = new(Category, 409);
    public static readonly StatusCode ProjectNotFound = new(Category, 404);
}
