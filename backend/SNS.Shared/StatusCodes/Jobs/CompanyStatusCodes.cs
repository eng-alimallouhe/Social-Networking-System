namespace SNS.Shared.StatusCodes.Jobs;

public static class CompanyStatusCodes
{
    private const string Category = "Companies";

    // Success
    public static readonly StatusCode CompanyUpdated = new(Category, 200);
    public static readonly StatusCode CompanyDeleted = new(Category, 200);

    // Errors
    public static readonly StatusCode CompanyNotFound = new(Category, 404);
    public static readonly StatusCode NotCompanyAdmin = new(Category, 403);
    public static readonly StatusCode CompanyNotActive = new(Category, 400);
}
