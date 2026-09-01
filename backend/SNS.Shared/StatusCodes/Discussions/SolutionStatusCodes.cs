namespace SNS.Shared.StatusCodes.Discussions;

public static class SolutionStatusCodes
{
    private const string Category = "Solutions";

    // Success
    public static readonly StatusCode SolutionCreated = new(Category, 201);
    public static readonly StatusCode SolutionUpdated = new(Category, 200);
    public static readonly StatusCode SolutionDeleted = new(Category, 200);
    public static readonly StatusCode SolutionStatusChanged = new(Category, 200);
    public static readonly StatusCode VoteAdded = new(Category, 201);
    public static readonly StatusCode VoteUpdated = new(Category, 200);
    public static readonly StatusCode VoteRemoved = new(Category, 200);

    // Errors
    public static readonly StatusCode SolutionNotFound = new(Category, 404);
    public static readonly StatusCode NotSolutionOwner = new(Category, 403);
    public static readonly StatusCode InvalidStatusTransition = new(Category, 400);
    public static readonly StatusCode ProblemClosed = new(Category, 400);
    public static readonly StatusCode VoteNotFound = new(Category, 404);
}
