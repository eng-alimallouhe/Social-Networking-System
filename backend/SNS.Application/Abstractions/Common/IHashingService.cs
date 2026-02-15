namespace SNS.Application.Abstractions.Common;

/// <summary>
/// Represents a domain service responsible for
/// cryptographic hashing and verification of sensitive data.
/// 
/// This service encapsulates the business logic related to
/// secure one-way hashing algorithms (e.g., BCrypt, Argon2), while keeping the Application layer
/// decoupled from infrastructure and implementation details.
/// </summary>
public interface IHashingService
{
    // ------------------------------------------------------------------
    // Operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Generates a secure cryptographic hash from the provided input.
    /// 
    /// This operation is responsible for:
    /// - Applying a specific hashing algorithm.
    /// - Generating a unique salt (if applicable) to protect against rainbow table attacks.
    /// - Producing a secure, irreversible representation of the input.
    /// </summary>
    /// <param name="input">
    /// The plain text value to be hashed (e.g., a password).
    /// </param>
    /// <returns>
    /// The resulting hashed string.
    /// </returns>
    string Hash(string input);

    /// <summary>
    /// Verifies that a plain text input matches a specific hashed value.
    /// 
    /// This operation is responsible for:
    /// - Hashing the input using the same parameters (salt/algorithm) as the stored hash.
    /// - Performing a secure comparison to validate the match.
    /// </summary>
    /// <param name="input">
    /// The plain text value provided for verification.
    /// </param>
    /// <param name="hash">
    /// The previously stored hash to compare against.
    /// </param>
    /// <returns>
    /// <c>true</c> if the input matches the hash; otherwise, <c>false</c>.
    /// </returns>
    bool Verify(string input, string hash);
}