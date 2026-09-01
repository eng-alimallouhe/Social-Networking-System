using SNS.Application.Preferences.Tags.Contracts;
using SNS.Application.Shared.Abstractions.Messaging;

namespace SNS.Application.Preferences.Tags.Queries.GetTags;

/// <summary>
/// Represents a read-only query to retrieve tags for autocomplete, optionally filtered by a search term.
/// </summary>
/// <param name="Search">Optional search keyword to filter tags by name.</param>
public sealed record GetTagsQuery(
    string? Search = null
) : IQuery<List<TagDto>>;
