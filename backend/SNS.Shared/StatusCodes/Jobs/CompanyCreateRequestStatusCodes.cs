namespace SNS.Shared.StatusCodes.Jobs;

public static class CompanyCreateRequestStatusCodes
{
    private const string Category = "CompanyCreateRequests";

    // Success
    public static readonly StatusCode RequestCreated = new(Category, 201);
    public static readonly StatusCode RequestCancelled = new(Category, 200);
    public static readonly StatusCode RequestApproved = new(Category, 200);
    public static readonly StatusCode RequestRejected = new(Category, 200);

    // Errors
    public static readonly StatusCode RequestNotFound = new(Category, 404);
    public static readonly StatusCode DuplicatePendingRequest = new(Category, 400);
    public static readonly StatusCode RequestNotPending = new(Category, 400);
    public static readonly StatusCode NotAuthorizedReviewer = new(Category, 403);
    public static readonly StatusCode NotRequestOwner = new(Category, 403);
}
