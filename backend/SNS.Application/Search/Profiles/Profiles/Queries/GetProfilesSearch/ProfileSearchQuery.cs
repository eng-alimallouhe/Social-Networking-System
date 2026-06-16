namespace SNS.Application.Search.Profiles.Profiles.Queries.GetProfilesSearch;

public class ProfileSearchQuery
{
    public string SearchTerm { get; set; } = string.Empty;
    public List<string> RequiredSkills { get; set; } = new();
    public Guid? CurrentProfileId { get; set; } // We need this to check the BlackList!
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
