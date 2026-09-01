using Microsoft.Extensions.Options;
using SNS.Application.Profiles.Profiles.abstractions;
using SNS.Application.Shared.Settings;

namespace SNS.Infrastructure.Profiles.Profiles.Abstractions;

public class ReputationPolicyService : IReputationPolicyService
{
    private readonly ReputationSettings _settings;

    public ReputationPolicyService(IOptionsSnapshot<ReputationSettings> settings)
    {
        _settings = settings.Value;
    }

    public ReputationTier GetUserLimits(int userPoints)
    {
        var tier = _settings.Tiers.FirstOrDefault(t => userPoints >= t.MinPoints && userPoints <= t.MaxPoints);

        return tier ?? _settings.Tiers.OrderBy(t => t.MinPoints).First();
    }

    public bool CanCreatePost(int userPoints, int postsCreatedToday)
    {
        var limits = GetUserLimits(userPoints);
        return postsCreatedToday < limits.MaxDailyPosts;
    }

    public bool CanCreateCV(int userPoints, int currentCvCount)
    {
        var limits = GetUserLimits(userPoints);
        return currentCvCount < limits.MaxAllowedCVs;
    }

    public bool CanCreateComment(int userPoints, int commentsCreatedToday)
    {
        var limits = GetUserLimits(userPoints);
        return commentsCreatedToday < limits.MaxDailyComments;
    }

    public bool CanCreateProblem(int userPoints, int problemsCreatedToday)
    {
        var limits = GetUserLimits(userPoints);
        return problemsCreatedToday < limits.MaxDailyProblems;
    }

    public bool CanCreateSolution(int userPoints, int solutionsCreatedToday)
    {
        var limits = GetUserLimits(userPoints);
        return solutionsCreatedToday < limits.MaxDailySolutions;
    }
}
