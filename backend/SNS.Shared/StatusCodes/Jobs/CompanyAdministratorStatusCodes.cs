namespace SNS.Shared.StatusCodes.Jobs;

public static class CompanyAdministratorStatusCodes
{
    private const string Category = "CompanyAdministrators";

    // Success
    public static readonly StatusCode AdminAdded = new(Category, 201);
    public static readonly StatusCode AdminRemoved = new(Category, 200);
    public static readonly StatusCode RoleChanged = new(Category, 200);

    // Errors
    public static readonly StatusCode AdminNotFound = new(Category, 404);
    public static readonly StatusCode AdminAlreadyExists = new(Category, 400);
    public static readonly StatusCode CannotRemoveSoleOwner = new(Category, 400);
    public static readonly StatusCode NotCompanyAdmin = new(Category, 403);
    public static readonly StatusCode ProfileNotFound = new(Category, 404);
    public static readonly StatusCode ProfileNotActive = new(Category, 400);
}
