using SNS.Application.DTOs.Authentication.Register;
using SNS.Application.DTOs.Authentication.Register.SNS.Domain.DTOs;
using SNS.Application.DTOs.Authentication.Responses; // For AuthResponse
using SNS.Common.Results;

namespace SNS.Application.Abstractions.Authentication;

/// <summary>
/// Defines a domain-level service responsible for handling user registration 
/// and account activation workflows.
/// 
/// This service acts as an orchestrator, coordinating between Repositories,
/// CodeService, PendingUpdateService, and TokenService to ensure a transactional
/// and secure onboarding process.
/// </summary>
public interface IRegisterService
{
    /// <summary>
    /// Initiates the registration process for a new user.
    /// 
    /// Logic:
    /// 1. Checks if the phone number is already taken by an active user.
    /// 2. Checks if the phone number is currently locked in a pending verification state.
    /// 3. Creates the User and Profile entities (Inactive State).
    /// 4. Creates a 'NewRegistration' pending update request for the phone number.
    /// 5. Dispatches a verification code linked to that pending request.
    /// </summary>
    /// <param name="dto">The registration payload (Name, Phone, Password).</param>
    /// <returns>
    /// A <see cref="Result{Guid}"/> containing the <see cref="Guid"/> of the newly created User.
    /// This ID is required for the subsequent activation step.
    /// </returns>
    Task<Result<Guid>> RegisterAsync(RegisterDto dto);

    /// <summary>
    /// Finalizes the registration by verifying the activation code and 
    /// transitioning the user to an Active state.
    /// 
    /// Logic:
    /// 1. Verifies the code against the PendingUpdate context.
    /// 2. Moves the phone number from PendingUpdate to the User entity.
    /// 3. Activates the user.
    /// 4. Creates a new Session, Archives Identity, and Archives Password.
    /// 5. Generates and returns Authentication Tokens.
    /// </summary>
    /// <param name="dto">Contains UserID and the Verification Code.</param>
    /// <returns>
    /// A <see cref="Result{AuthenticationResultDto}"/> containing the Access and Refresh tokens.
    /// </returns>
    Task<Result<AuthenticationResultDto>> ActiveAccountAsync(AccountActivationDto dto);
}