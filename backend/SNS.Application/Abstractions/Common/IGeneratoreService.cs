namespace SNS.Application.Abstractions.Common;

/// <summary>
/// Defines a service responsible for generating secure, random values
/// used for security purposes.
/// 
/// This abstraction allows for different generation strategies (e.g.,
/// numeric codes, alphanumeric tokens) without coupling the domain logic
/// to a specific random number generator implementation.
/// </summary>
public interface IGeneratorService
{
    /// <summary>
    /// Generates a secure, random string suitable for use as a
    /// verification code or one-time password (OTP).
    /// </summary>
    /// <returns>
    /// A string containing the generated code (e.g., a 6-digit number).
    /// </returns>
    string GenerateSecureCode();


    /// <summary>
    /// Generates a cryptographically secure random string (Base64 encoded).
    /// Used for Refresh Tokens, API Keys, or random salts.
    /// </summary>
    /// <param name="byteLength">The number of random bytes to generate (Default is 32).</param>
    /// <returns>A Base64 string representation of the random bytes.</returns>
    string GenerateSecureString(int byteLength = 32);

    /// <summary>
    /// Generates a secure, random string suitable for use as a
    /// Security Code
    /// </summary>
    /// <returns>
    /// A string containing the generated code (e.g., a 32-digit characters).
    /// </returns>

    string GenerateSecretKey();
}