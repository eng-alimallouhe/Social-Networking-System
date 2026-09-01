namespace SNS.Shared.StatusCodes.Jobs;

public static class JobSkillStatusCodes
{
    private const string Category = "JobSkills";

    // Success
    public static readonly StatusCode JobSkillAdded = new(Category, 201);
    public static readonly StatusCode JobSkillRemoved = new(Category, 200);

    // Errors
    public static readonly StatusCode JobSkillNotFound = new(Category, 404);
    public static readonly StatusCode JobSkillAlreadyExists = new(Category, 400);
    public static readonly StatusCode SkillNotFound = new(Category, 404);
    public static readonly StatusCode JobNotFound = new(Category, 404);
    public static readonly StatusCode NotCompanyAdmin = new(Category, 403);
}
