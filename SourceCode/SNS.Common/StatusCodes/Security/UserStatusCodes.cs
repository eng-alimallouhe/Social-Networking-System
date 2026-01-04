namespace SNS.Common.StatusCodes.Security;

/// <summary>
/// Defines status codes related to user account states and validation.
/// Uses the "User" category for translation keys.
/// </summary>
public static class UserStatusCodes
{
    private const string Category = "User";

    /// <summary>
    /// Indicates that the reque1sted user profile or account created.
    /// <para>HTTP Equivalent: 20 Created</para>
    /// </summary>
    public static readonly StatusCode Created =
        new(Category, 201);



    
    /// <summary>
    /// Indicates that the requested user profile or account founded.
    /// <para>HTTP Equivalent: 200 Ok or Found</para>
    /// </summary>
    public static readonly StatusCode Found =
        new(Category, 200);

    /// <summary>
    /// Indicates that the requested user profile or account could not be found.
    /// <para>HTTP Equivalent: 404 Not Found</para>
    /// </summary>
    public static readonly StatusCode NotFound =
        new(Category, 404);

    /// <summary>
    /// Indicates that a user with the same identifier (Email/Phone/Username) already exists.
    /// <para>HTTP Equivalent: 409 Conflict</para>
    /// </summary>
    public static readonly StatusCode AlreadyExists =
        new(Category, 409);
    
    /// <summary>
    /// Indicates that the user account is inactive (Soft Deleted) or disabled by the user.
    /// <para>HTTP Equivalent: 410 Gone (or 403)</para>
    /// </summary>
    public static readonly StatusCode Inactive =
        new(Category, 403);

    /// <summary>
    /// Indicates that the user account is permanently banned due to violations.
    /// <para>HTTP Equivalent: 403 Forbidden</para>
    /// </summary>
    public static readonly StatusCode Banned =
        new(Category, 4031); // 403 + 1 (Custom suffix for frontend logic)

    /// <summary>
    /// Indicates that the user account is temporarily suspended.
    /// <para>HTTP Equivalent: 403 Forbidden</para>
    /// </summary>
    public static readonly StatusCode Suspended =
        new(Category, 4032);


    /// <summary>
    /// Indicates that the user has not verified their email or phone number yet.
    /// <para>HTTP Equivalent: 403 Forbidden</para>
    /// </summary>
    public static readonly StatusCode NotVerified =
        new(Category, 4033); // 403 + 3

    /// <summary>
    /// Indicates that the user has verified their account but has not completed their profile creation.
    /// The frontend should redirect the user to the profile building wizard.
    /// <para>HTTP Equivalent: 403 Forbidden (Sub-code: 4)</para>
    /// </summary>
    public static readonly StatusCode ProfileNotCompleted =
        new(Category, 4034);

    /// <summary>
    /// when user request to recivery her account by security code and 
    /// there are active session in the current time 
    /// </summary>
    public static readonly StatusCode InvalidSecurityUse =
        new(Category, 404);
}