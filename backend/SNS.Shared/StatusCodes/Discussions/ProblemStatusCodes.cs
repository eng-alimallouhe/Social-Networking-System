namespace SNS.Shared.StatusCodes.Discussions;

public static class ProblemStatusCodes
{
    private const string Category = "Problems";

    // Success
    public static readonly StatusCode ProblemCreated = new(Category, 201);
    public static readonly StatusCode ProblemUpdated = new(Category, 200);
    public static readonly StatusCode ProblemDeleted = new(Category, 200);
    public static readonly StatusCode ProblemStatusChanged = new(Category, 200);
    public static readonly StatusCode VoteAdded = new(Category, 201);
    public static readonly StatusCode VoteUpdated = new(Category, 200);
    public static readonly StatusCode VoteRemoved = new(Category, 200);
    public static readonly StatusCode TagAdded = new(Category, 201);
    public static readonly StatusCode TagRemoved = new(Category, 200);
    public static readonly StatusCode ViewRecorded = new(Category, 200);

    // Errors
    public static readonly StatusCode ProblemNotFound = new(Category, 404);
    public static readonly StatusCode NotProblemOwner = new(Category, 403);
    public static readonly StatusCode ProblemClosed = new(Category, 400);
    public static readonly StatusCode InvalidStatusTransition = new(Category, 400);
    public static readonly StatusCode TagNotFound = new(Category, 404);
    public static readonly StatusCode TagAlreadyExists = new(Category, 400);
    public static readonly StatusCode VoteNotFound = new(Category, 404);
}
