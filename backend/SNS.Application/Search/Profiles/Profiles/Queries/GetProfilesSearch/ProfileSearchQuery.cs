namespace SNS.Application.Search.Profiles.Profiles.Queries.GetProfilesSearch;

/// <summary>
/// Represents filter parameters to search user profiles in the search index.
/// </summary>
public class ProfileSearchQuery
{
    /// <summary>
    /// Gets or sets the search term keyword matching full name, bio, or specialization.
    /// </summary>
    public string SearchTerm { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of required skill names to filter profiles.
    /// </summary>
    public List<string> RequiredSkills { get; set; } = new();

    /// <summary>
    /// Gets or sets the current user profile ID to exclude blocked relationships.
    /// </summary>
    public Guid? CurrentProfileId { get; set; }

    /// <summary>
    /// Gets or sets the page number for pagination (1-based).
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Gets or sets the maximum number of profiles returned per page.
    /// </summary>
    public int PageSize { get; set; } = 10;
}

