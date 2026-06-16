namespace SNS.Shared.StatusCodes.Identity;

public class SessionStatusCodes
{
    private const string Category = "Session";

    public static readonly StatusCode NotFound =
        new(Category, 404);
}
