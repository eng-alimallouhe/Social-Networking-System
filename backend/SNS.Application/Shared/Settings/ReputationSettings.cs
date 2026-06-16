namespace SNS.Application.Shared.Settings;

public class ReputationSettings
{
    public List<ReputationTier> Tiers { get; set; } = new();
}

public class ReputationTier
{
    public string Name { get; set; } = string.Empty;
    public int MinPoints { get; set; }
    public int MaxPoints { get; set; }
    public int MaxDailyPosts { get; set; }
    public int MaxDailyComments { get; set; }
    public int MaxAllowedCVs { get; set; }
}
