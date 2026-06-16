namespace SNS.Shared.StatusCodes.Identity;

/// <summary>
/// Defines status codes related to verification workflows
/// such as sending, validating, and throttling verification codes.
/// 
/// These status codes are used to describe the outcome of
/// verification-related operations in a consistent and
/// domain-oriented manner.
/// </summary>
public static class VerificationStatusCodes
{
    /// <summary>
    /// Indicates that a verification code was successfully sent
    /// to the user.
    /// </summary>
    public static readonly StatusCode CodeSent =
        new("Verification", 201);

    /// <summary>
    /// Indicates that the verification code was successfully
    /// validated and accepted.
    /// </summary>
    public static readonly StatusCode CodeVerified =
        new("Verification", 200);

    ///<summary>
    ///Indicates that code resend operation was successful.
    /// </summary>
    public static readonly StatusCode CodeResent =
        new("Verification", 202);

    /// <summary>
    /// Indicates that no active verification code exists
    /// for the requested operation.
    /// </summary>
    public static readonly StatusCode NoActiveCode =
        new("Verification", 404);

    /// <summary>
    /// Indicates that the verification code has expired
    /// and is no longer valid.
    /// </summary>
    public static readonly StatusCode CodeExpired =
        new("Verification", 410);

    /// <summary>
    /// Indicates that the provided verification code is invalid.
    /// </summary>
    public static readonly StatusCode InvalidCode =
        new("Verification", 400);

    /// <summary>
    /// Indicates that the maximum number of allowed verification
    /// attempts has been reached.
    /// </summary>
    public static readonly StatusCode MaxAttemptsReached =
        new("Verification", 429);

    /// <summary>
    /// Indicates that verification requests are temporarily
    /// throttled and the user must wait approximately
    /// 10 minutes before retrying.
    /// </summary>
    public static readonly StatusCode Throttled =
        new("Verification", 4291);
}
