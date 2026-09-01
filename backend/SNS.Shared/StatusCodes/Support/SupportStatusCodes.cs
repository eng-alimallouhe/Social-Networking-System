namespace SNS.Shared.StatusCodes.Support;

public static class SupportStatusCodes
{
    private const string Category = "Support";

    // Success (2xx)
    public static readonly StatusCode TicketCreated = new(Category, 201);
    public static readonly StatusCode TicketAssigned = new(Category, 200);
    public static readonly StatusCode PriorityChanged = new(Category, 200);
    public static readonly StatusCode StatusChanged = new(Category, 200);
    public static readonly StatusCode ReplyAdded = new(Category, 201);

    // Errors (4xx)
    public static readonly StatusCode TicketNotFound = new(Category, 404);
    public static readonly StatusCode NotTicketOwner = new(Category, 403);
    public static readonly StatusCode TicketClosed = new(Category, 400);
    public static readonly StatusCode InvalidAgent = new(Category, 400);
    public static readonly StatusCode UnauthorizedAccess = new(Category, 403);
}
