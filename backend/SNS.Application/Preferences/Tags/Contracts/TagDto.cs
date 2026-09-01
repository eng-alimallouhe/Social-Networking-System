namespace SNS.Application.Preferences.Tags.Contracts;

/// <summary>
/// Represents a lightweight data transfer object containing essential tag information for autocomplete and dropdowns.
/// </summary>
/// <param name="Id">The unique identifier of the tag.</param>
/// <param name="Name">The name of the tag.</param>
public sealed record TagDto(
    Guid Id,
    string Name
);
