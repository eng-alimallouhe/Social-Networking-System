namespace SNS.Shared.StatusCodes.Jobs;

public static class JobStatusCodes
{
    private const string Category = "Jobs";

    // Success
    public static readonly StatusCode JobCreated = new(Category, 201);
    public static readonly StatusCode JobUpdated = new(Category, 200);
    public static readonly StatusCode JobDeleted = new(Category, 200);
    public static readonly StatusCode JobClosed = new(Category, 200);

    // Errors
    public static readonly StatusCode JobNotFound = new(Category, 404);
    public static readonly StatusCode CompanyNotActive = new(Category, 400);
    public static readonly StatusCode NotCompanyAdmin = new(Category, 403);
    public static readonly StatusCode JobAlreadyClosed = new(Category, 400);
    public static readonly StatusCode InvalidSalaryRange = new(Category, 400);
}
