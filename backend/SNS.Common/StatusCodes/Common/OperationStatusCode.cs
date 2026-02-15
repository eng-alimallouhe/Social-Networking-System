namespace SNS.Common.StatusCodes.Common;

/// <summary>
/// Defines standard status codes for general system operations.
/// All codes in this class belong to the "Operation" category.
/// </summary>
public static class OperationStatusCode
{
    // Define the category once to avoid magic strings and ensure consistency.
    private const string Category = "Operation";

    /// <summary>
    /// Indicates that the operation completed successfully.
    /// <para>HTTP Equivalent: 200 OK</para>
    /// </summary>
    public static readonly StatusCode Success =
        new(Category, 200);

    /// <summary>
    /// Indicates a general failure due to business logic or invalid state.
    /// <para>HTTP Equivalent: 400 Bad Request</para>
    /// </summary>
    public static readonly StatusCode Failure =
        new(Category, 400);

    /// <summary>
    /// Indicates that the user is not authenticated and needs to log in.
    /// <para>HTTP Equivalent: 401 Unauthorized</para>
    /// </summary>
    public static readonly StatusCode AuthenticationRequired =
        new(Category, 401);

    /// <summary>
    /// Indicates that the user is authenticated but lacks the necessary permissions.
    /// <para>HTTP Equivalent: 403 Forbidden</para>
    /// </summary>
    public static readonly StatusCode AccessDenied =
        new(Category, 403);

    /// <summary>
    /// Indicates that the requested resource (e.g., entity, file) was not found.
    /// <para>HTTP Equivalent: 404 Not Found</para>
    /// </summary>
    public static readonly StatusCode ResourceNotFound =
        new(Category, 404);

    /// <summary>
    /// Indicates a conflict with the current state of the resource (e.g., duplicate entry).
    /// <para>HTTP Equivalent: 409 Conflict</para>
    /// </summary>
    public static readonly StatusCode Conflict =
        new(Category, 409);

    /// <summary>
    /// Indicates that the provided input data is invalid or unprocessable.
    /// <para>HTTP Equivalent: 422 Unprocessable Entity</para>
    /// </summary>
    public static readonly StatusCode InvalidInput =
        new(Category, 422);

    /// <summary>
    /// Indicates an unexpected internal server error.
    /// <para>HTTP Equivalent: 500 Internal Server Error</para>
    /// </summary>
    public static readonly StatusCode ServerError =
        new(Category, 500);
}