using SNS.Domain.Security;

namespace SNS.Application.DTOs.Authentication.Verification.Requests
{
    /// <summary>
    /// Represents the data required to verify a security code
    /// sent to a user.
    /// 
    /// This object encapsulates the user's identifier, the
    /// code itself, and the context (type) of the code being verified.
    /// </summary>
    public class VerifyCodeDto
    {
        /// <summary>
        /// The unique identifier of the user attempting verification.
        /// <br/>
        /// This could be an email address or a phone number, depending
        /// on the registration method or the specific flow.
        /// </summary>
        public string UserIdentifier { get; set; } = string.Empty;

        /// <summary>
        /// The actual verification code received by the user.
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// The purpose or category of the code being verified
        /// (e.g., EmailConfirmation, PasswordReset).
        /// 
        /// This ensures that a code generated for one purpose cannot
        /// be maliciously used for another.
        /// </summary>
        public CodeType CodeType { get; set; }
    }
}
