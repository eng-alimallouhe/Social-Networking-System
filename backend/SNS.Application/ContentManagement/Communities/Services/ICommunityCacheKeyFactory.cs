namespace SNS.Application.ContentManagement.Communities.Services;

public interface ICommunityCacheKeyFactory
{
    string GetCommunityMembersKey(Guid communityId);
    string GetTrendingCommunitiesKey(DateTime date);
}
