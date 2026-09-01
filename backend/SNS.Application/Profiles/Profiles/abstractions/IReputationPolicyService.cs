using SNS.Application.Shared.Settings;

namespace SNS.Application.Profiles.Profiles.abstractions;

public interface IReputationPolicyService
{
    ReputationTier GetUserLimits(int userPoints);

    bool CanCreatePost(int userPoints, int postsCreatedToday);
    bool CanCreateCV(int userPoints, int currentCvCount);
    bool CanCreateProblem(int userPoints, int problemsCreatedToday);
    bool CanCreateSolution(int userPoints, int solutionsCreatedToday);
    bool CanCreateComment(int userPoints, int commentsCreatedToday);
}
