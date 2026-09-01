namespace SNS.Shared.StatusCodes.Jobs;

public static class JobApplicationStatusCodes
{
    private const string Category = "JobApplications";

    // Success
    public static readonly StatusCode ApplicationCreated = new(Category, 201);
    public static readonly StatusCode ApplicationWithdrawn = new(Category, 200);
    public static readonly StatusCode StatusUpdated = new(Category, 200);

    // Errors
    public static readonly StatusCode ApplicationNotFound = new(Category, 404);
    public static readonly StatusCode DuplicateApplication = new(Category, 400);
    public static readonly StatusCode JobClosedOrDeleted = new(Category, 400);
    public static readonly StatusCode ResumeNotFound = new(Category, 404);
    public static readonly StatusCode NotResumeOwner = new(Category, 403);
    public static readonly StatusCode NotApplicant = new(Category, 403);
    public static readonly StatusCode NotCompanyAdmin = new(Category, 403);
    public static readonly StatusCode InvalidStatusTransition = new(Category, 400);
}
