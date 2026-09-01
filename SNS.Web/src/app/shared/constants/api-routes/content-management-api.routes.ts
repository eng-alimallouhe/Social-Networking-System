export const CONTENT_MANAGEMENT_API_ROUTES = {
    // Posts
    Posts: 'content-managment/Posts',
    Feed: 'content-managment/Posts/feed',
    PostById: (postId: string) => `content-managment/Posts/${postId}`,
    ReactedPosts: 'content-managment/Posts/reacted',
    UserPosts: (profileId: string) => `content-managment/Posts/user/${profileId}`,
    IncreasePostInterest: (postId: string) => `content-managment/Posts/${postId}/interest/increase`,
    DecreasePostInterest: (postId: string) => `content-managment/Posts/${postId}/interest/decrease`,

    PostReactions: (postId: string) => `content-managment/posts/${postId}/reactions`,

    SavedPosts: 'content-managment/posts/saved',
    SavePost: (postId: string) => `content-managment/posts/${postId}/save`,
    UnsavePost: (postId: string) => `content-managment/posts/${postId}/save`,

    // Comments
    Comments: 'content-managment/Comments',
    CommentById: (commentId: string) => `content-managment/Comments/${commentId}`,
    PostComments: (postId: string) => `content-managment/Comments/post/${postId}`,
    CommentReplies: (commentId: string) => `content-managment/Comments/${commentId}/replies`,
    UserComments: (profileId: string) => `content-managment/Comments/user/${profileId}`,
    MyComments: 'content-managment/Comments/my-comments',
    CommentReactions: (commentId: string) => `content-managment/comments/${commentId}/reactions`,

    // Communities
    Communities: 'content-managment/communities',
    CommunityById: (id: string) => `content-managment/communities/${id}`,
    MyCommunities: 'content-managment/communities/my-communities',

    // Community Memberships
    JoinCommunity: (communityId: string) => `content-managment/communities/${communityId}/memberships/join`,
    LeaveCommunity: (communityId: string) => `content-managment/communities/${communityId}/memberships/leave`,
    CommunityMembers: (communityId: string) => `content-managment/communities/${communityId}/memberships/members`,
    MembershipRequests: (communityId: string) => `content-managment/communities/${communityId}/memberships/requests`,
    ApproveMembershipRequest: (communityId: string, requestId: string) => `content-managment/communities/${communityId}/memberships/requests/${requestId}/approve`,
    RejectMembershipRequest: (communityId: string, requestId: string) => `content-managment/communities/${communityId}/memberships/requests/${requestId}/reject`,
    RemoveCommunityMember: (communityId: string, memberProfileId: string) => `content-managment/communities/${communityId}/memberships/members/${memberProfileId}`,
    ChangeCommunityMemberRole: (communityId: string, memberProfileId: string) => `content-managment/communities/${communityId}/memberships/members/${memberProfileId}/role`,
    MyCommunityMembershipStatus: (communityId: string) => `content-managment/communities/${communityId}/memberships/my-status`,

    // Community Settings
    CommunitySettings: (communityId: string) => `content-managment/communities/${communityId}/settings`,

    // Community Rules
    CommunityRules: (communityId: string) => `content-managment/communities/${communityId}/rules`,
    CommunityRuleById: (communityId: string, ruleId: string) => `content-managment/communities/${communityId}/rules/${ruleId}`,

    // Community Trending
    TrendingCommunities: 'content-managment/communities/trending',
} as const;
