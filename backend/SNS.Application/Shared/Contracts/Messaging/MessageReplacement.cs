using SNS.Domain.Identity.Shared.Enums;

namespace SNS.Application.Shared.Contracts.Messaging;

/// <summary>
/// Represents a message template placeholder replacement key and value pair.
/// </summary>
/// <param name="Key">The template placeholder key enum value.</param>
/// <param name="Value">The text value to substitute into the template.</param>
public sealed record MessageReplacement(
    ReplacementKey Key,
    string Value);

