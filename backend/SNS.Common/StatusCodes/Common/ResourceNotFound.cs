namespace SNS.Common.StatusCodes.Common;

/// <summary>
/// Defines status codes related to system resources (Files, Templates, Static Assets).
/// This distinguishes physical resource errors from business logic errors.
/// </summary>
public static class ResourceStatusCode
{
    // Category is fixed to "Resource" for translation keys (e.g., RESOURCE_404)
    private const string Category = "Resource";

    /// <summary>
    /// Indicates that the requested resource (file/template) was found successfully.
    /// <para>HTTP Equivalent: 200 OK</para>
    /// </summary>
    public static readonly StatusCode Found =
        new(Category, 200);

    /// <summary>
    /// Indicates that the physical resource (file/template) does not exist on the server.
    /// <para>HTTP Equivalent: 404 Not Found</para>
    /// </summary>
    public static readonly StatusCode NotFound =
        new(Category, 404);

    /// <summary>
    /// Indicates an error occurred while trying to read the resource (e.g., Permission denied, Corrupt file).
    /// <para>HTTP Equivalent: 500 Internal Server Error</para>
    /// </summary>
    public static readonly StatusCode ReadError =
        new(Category, 500);
}