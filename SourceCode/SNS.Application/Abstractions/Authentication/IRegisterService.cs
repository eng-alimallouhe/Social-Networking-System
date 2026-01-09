using SNS.Application.DTOs.Authentication.Register;
using SNS.Application.DTOs.Authentication.Common.Responses;
using SNS.Application.DTOs.Authentication.Register.Reponses;
using SNS.Application.DTOs.Authentication.Register.Requests;
using SNS.Common.Results;

namespace SNS.Application.Abstractions.Authentication;

/// <summary>
/// Represents a domain service responsible for
/// handling user registration and account activation workflows.
/// 
/// This service encapsulates the business logic related to
/// onboarding new users, orchestrating entity creation, and managing the transition
/// from inactive to active states, while keeping the Application layer
/// decoupled from infrastructure and implementation details.
/// </summary>
public interface IRegisterService
{
    // ------------------------------------------------------------------
    // Command operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Initiates the registration process for a new user.
    /// 
    /// This operation is responsible for:
    /// - Validating that the provided phone number is unique and not currently locked.
    /// - Creating the initial User and Profile entities in an inactive state.
    /// - Initiating the verification workflow by creating a pending update request.
    /// - Dispatching a verification code to the user.
    /// </summary>
    /// <param name="dto">
    /// The registration payload containing the user's basic information (Name, Phone, Password).
    /// </param>
    /// <returns>
    /// A <see cref="Result{RegisterResponseDto}"/> containing the newly created User's ID
    /// and security context if the operation completed successfully; otherwise, a failure result.
    /// </returns>
    Task<Result<RegisterResponseDto>> RegisterAsync(
        RegisterDto dto);

    /// <summary>
    /// Finalizes the registration by verifying the activation code and transitioning the user to an active state.
    /// 
    /// This operation is responsible for:
    /// - Validating the provided code against the pending registration request.
    /// - Activating the user account and confirming the phone number.
    /// - Establishing the initial security context (Session, Password Archive).
    /// - Generating and returning the initial set of authentication tokens.
    /// </summary>
    /// <param name="dto">
    /// The data transfer object containing the User ID and the Verification Code.
    /// </param>
    /// <returns>
    /// A <see cref="Result{AuthTokensDto}"/> containing the Access and Refresh tokens
    /// if the activation is successful; otherwise, a failure result.
    /// </returns>
    Task<Result<AuthTokensDto>> ActiveAccountAsync(
        AccountActivationDto dto);

    /// <summary>
    /// Re-initiates the activation request for a user who has not yet completed registration.
    /// 
    /// This operation is responsible for:
    /// - Resending the activation code to users who may have lost the previous one.
    /// - Ensuring the account is still in a valid state to receive a new activation request.
    /// </summary>
    /// <param name="dto">
    /// The data transfer object identifying the pending activation request.
    /// </param>
    /// <returns>
    /// A <see cref="Result{RegisterResponseDto}"/> confirming the request was processed
    /// and the notification has been re-queued.
    /// </returns>
    Task<Result<RegisterResponseDto>> ResendActiveRequestAsync(
        ResendActiveRequestDto dto);
}