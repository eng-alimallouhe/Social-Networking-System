using SNS.Application.ContentManagement.Communities.Services;
using SNS.Application.ContentManagement.Posts.Abstractions;
using SNS.Application.Identity.Shared.Abstractions;
using SNS.Application.Profiles.Profiles.abstractions;
using SNS.Domain.Identity.Users.Enums;

namespace SNS.Infrastructure.Shared.Services.Cashing;

public class CacheKeyFactory : 
    IIdentityCacheKeyFactory, 
    IProfileCacheKeyFactory,
    ICommunityCacheKeyFactory,
    IPostCacheKeyFactory
{
    public string GetUserKey(Guid userId)
        => $"user:{userId}";

    public string GetUserPostsKey(Guid userId)
        => $"user:{userId.ToString()}:posts";

    public string GetPostKey(Guid postId)
        => $"post:{postId.ToString()}";

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
}
