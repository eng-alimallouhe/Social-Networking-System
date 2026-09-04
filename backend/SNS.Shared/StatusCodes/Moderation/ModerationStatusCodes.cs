namespace SNS.Shared.StatusCodes.Moderation;

public static class ModerationStatusCodes
{
    private const string Category = "Moderation";

    // Success (2xx)
    public static readonly StatusCode ReportSubmitted = new(Category, 201);
    public static readonly StatusCode TicketResolved = new(Category, 200);

    // Errors (4xx)
    public static readonly StatusCode TargetNotFound = new(Category, 404);
    public static readonly StatusCode TicketNotFound = new(Category, 404);
    public static readonly StatusCode AlreadyReported = new(Category, 409);
    public static readonly StatusCode UnauthorizedAccess = new(Category, 403);
}
