using SNS.Application.ContentManagement.Communities.Services;
using SNS.Application.ContentManagement.Posts.Posts.Abstractions;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.abstractions;
using SNS.Domain.Identity.Users.Enums;
using SNS.Application.Projects.Abstractions;

namespace SNS.Infrastructure.Shared.Services.Cashing;

public class CacheKeyFactory :
    IIdentityCacheKeyFactory,
    IProfileCacheKeyFactory,
    ICommunityCacheKeyFactory,
    IPostCacheKeyFactory,
    IProjectCacheKeyFactory
{
    public string GetUserKey(Guid userId)
        => $"user:{userId}";

    public string GetProfileFeedKey(Guid profileId)
        => $"profile:feed:{profileId.ToString()}";

    public string GetProjectProfileFeedKey(Guid profileId)
        => $"profile:project-feed:{profileId.ToString()}";

    public string GetCelebrityPostKey(Guid publisherId)
        => $"celebrity:posts:{publisherId.ToString()}";

    public string GetCommunityMembersKey(Guid communityId)
        => $"community:{communityId.ToString()}:members";

    public string GetSessionKey(Guid sessionId)
        => $"session:{sessionId.ToString()}";

    public string GetUserSessionsKey(Guid userId)
        => $"user:sessions:{userId}";

    public string GetOtpKey(Guid userId)
        => $"auth:tfa:code:{userId}";

    public string GetUserActivationChanlageKey(Guid userId) =>
        $"user:{userId}:activation-chanlage";

    public string GetCoolDownKey(Guid userId)
        => $"auth:tfa:cooldown:{userId}";

    public string GetAttemptsKey(Guid userId)
        => $"auth:tfa:attempts:{userId}";

    public string GetUpdateKey(Guid userId, UpdateType type)
    {
        return type switch
        {
            UpdateType.Email => $"user:{userId}:update-email",
            UpdateType.Password => $"user:{userId}:update-password",
            _ => $"user:{userId}:update-phone"
        };
    }

    public string GetUserProfileMappingKey(Guid userId)
        => $"user:profile:mapping:{userId}";

    public string GetProfileKey(Guid profileId)
        => $"profile:{profileId}";


    public string GetTrendingCommunitiesKey(DateTime date)
        => $"trending:communities:{date:UtcNow:yyyy-MM-dd}";

    public string GetFeedBuildingKey(Guid profileId)
        => $"feed:building:profileId:{profileId}";

    public string GetUserAuthenticatorKey(Guid userId)
        => $"user:{userId}:authenticator-setup";

    public string GetRolePermissionsMatrixKey()
        => "identity:role-permissions:matrix";
}
