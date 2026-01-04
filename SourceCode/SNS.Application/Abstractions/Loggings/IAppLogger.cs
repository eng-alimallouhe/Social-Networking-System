namespace SNS.Application.Abstractions.Loggings;

/// <summary>
/// Represents a domain service responsible for
/// capturing and recording application runtime events, warnings, and errors.
/// 
/// This service encapsulates the business logic related to
/// structured logging and diagnostics, while keeping the Application layer
/// decoupled from infrastructure and implementation details.
/// </summary>
/// <typeparam name="TEntity">
/// The type of the entity or class context for which logs are being generated.
/// </typeparam>
public interface IAppLogger<TEntity>
{
    // ------------------------------------------------------------------
    // Operations
    // ------------------------------------------------------------------

    /// <summary>
    /// Records an informational message regarding the application flow.
    /// 
    /// This operation is responsible for:
    /// - Capturing standard operational events.
    /// - Formatting the message with the provided arguments.
    /// </summary>
    /// <param name="message">
    /// The message template describing the event.
    /// </param>
    /// <param name="args">
    /// Optional arguments to structure the message.
    /// </param>
    void LogInformation(string message, params object[] args);

    /// <summary>
    /// Records a warning message indicating a potential issue or unexpected state.
    /// 
    /// This operation is responsible for:
    /// - Capturing non-critical anomalies that do not stop execution.
    /// - Alerting administrators to potential future problems.
    /// </summary>
    /// <param name="message">
    /// The message template describing the warning.
    /// </param>
    /// <param name="args">
    /// Optional arguments to structure the message.
    /// </param>
    void LogWarning(string message, params object[] args);

    /// <summary>
    /// Records an error message indicating a failure in the application execution.
    /// 
    /// This operation is responsible for:
    /// - Capturing exceptions and critical failures.
    /// - Preserving stack traces and exception details for debugging.
    /// </summary>
    /// <param name="message">
    /// The message template describing the error.
    /// </param>
    /// <param name="exception">
    /// The exception object associated with the error.
    /// </param>
    /// <param name="args">
    /// Optional arguments to structure the message.
    /// </param>
    void LogError(string message, Exception exception, params object[] args);
}