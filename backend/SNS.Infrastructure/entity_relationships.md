# Entity Relationships in SNS.Infrastructure

This document outlines all entity relationships and their configured deletion behaviors across the various modules in the `SNS.Infrastructure` layer.

---

## 1. Identity Module

| Principal Entity | Dependent Entity | Navigation (Principal to Dependent) | Navigation (Dependent to Principal) | Relationship Type | Deletion Behavior | Configured In |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Role` | `User` | *None* | `User.Role` | One-to-Many | Default (Cascade) | [UserConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Identity/Users/Configurations/UserConfigurations.cs) |
| `User` | `Profile` | `User.UserProfile` | `Profile.Owner` | One-to-One | Default (Cascade) | [ProfileConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Profiles/Profiles/Configurations/ProfileConfigurations.cs) |
| `User` | `ExportDataRequest` | *None* | *None* | One-to-Many | Cascade | [ExportDataRequestConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Identity/ArchiveManagement/Configurations/ExportDataRequestConfigurations.cs) |
| `User` | `IdentityArchive` | `User.IdentityArchives` | *None* | One-to-Many | Default (Cascade) | [IdentityArchiveConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Identity/ArchiveManagement/Configurations/IdentityArchiveConfigurations.cs) |
| `User` | `PasswordArchive` | `User.PasswordArchives` | *None* | One-to-Many | Default (Cascade) | [PasswordArchiveConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Identity/ArchiveManagement/Configurations/PasswordArchiveConfigurations.cs) |
| `User` (Target) | `UserArchive` | `User.Archives` | *None* | One-to-Many | Restrict | [UserArchiveConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Identity/ArchiveManagement/Configurations/UserArchiveConfigurations.cs) |
| `User` (Actor) | `UserArchive` | `User.ActionPerformed` | *None* | One-to-Many (Optional) | Restrict | [UserArchiveConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Identity/ArchiveManagement/Configurations/UserArchiveConfigurations.cs) |
| `User` | `Notification` | `User.Notifications` | *None* | One-to-Many | Default (Cascade) | [NotificationConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Identity/Notifications/Configurations/NotificationConfigurations.cs) |
| `User` | `UserNotificationPreferences` | `User.NotificationPreferences` | *None* | One-to-One | Cascade | [UserNotificationPreferencesConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Identity/Notifications/Configurations/UserNotificationPreferencesConfigurations.cs) |
| `User` | `Device` | `User.Devices` | *None* | One-to-Many | Cascade | [DeviceConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Identity/SecuritySessions/Configurations/DeviceConfigurations.cs) |
| `SecuritySession` | `RefreshToken` | `SecuritySession.RefreshTokens` | *None* | One-to-Many | Cascade | [SecuritySessionConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Identity/SecuritySessions/Configurations/SecuritySessionConfigurations.cs) / [RefreshTokenConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Identity/SecuritySessions/Configurations/RefreshTokenConfigurations.cs) |
| `User` | `SecuritySession` | `User.Sessions` | *None* | One-to-Many | Restrict | [SecuritySessionConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Identity/SecuritySessions/Configurations/SecuritySessionConfigurations.cs) |
| `Device` | `SecuritySession` | `Device.Sessions` | `SecuritySession.Device` | One-to-Many | Default (Cascade) | [SecuritySessionConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Identity/SecuritySessions/Configurations/SecuritySessionConfigurations.cs) |
| `UserSecuritySettings` | `RecoveryCode` | `UserSecuritySettings.RecoveryCodes` | *None* | One-to-Many | Restrict | [RecoveryCodeConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Identity/SecuritySettings/Configurations/RecoveryCodeConfiguration.cs) |
| `User` | `UserPasskey` | `User.Passkeys` | *None* | One-to-Many | Default (Cascade) | [UserPasskeyConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Identity/SecuritySettings/Configurations/UserPasskeyConfigurations.cs) |
| `User` | `UserSecuritySettings` | `User.UserSecuritySettings` | *None* | One-to-One | Restrict | [UserSecuritySettingsConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Identity/SecuritySettings/Configurations/UserSecuritySettingsConfiguration.cs) |

---

## 2. Profiles Module

| Principal Entity | Dependent Entity | Navigation (Principal to Dependent) | Navigation (Dependent to Principal) | Relationship Type | Deletion Behavior | Configured In |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Profile` | `ProfileSkill` | `Profile.ProfileSkills` | *None* | One-to-Many | Cascade | [ProfileSkillConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Profiles/Profiles/Configurations/ProfileSkillConfigurations.cs) |
| `Skill` | `ProfileSkill` | *None* | `ProfileSkill.Skill` | One-to-Many | Cascade | [ProfileSkillConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Profiles/Profiles/Configurations/ProfileSkillConfigurations.cs) |
| `Profile` | `ProfileTopic` | `Profile.ProfileTopics` | *None* | One-to-Many | Cascade | [ProfileTopicConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Profiles/Profiles/Configurations/ProfileTopicConfigurations.cs) |
| `Topic` | `ProfileTopic` | *None* | *None* | One-to-Many | Cascade | [ProfileTopicConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Profiles/Profiles/Configurations/ProfileTopicConfigurations.cs) |
| `Profile` (Viewer) | `ProfileView` | `Profile.Views` | *None* | One-to-Many | Restrict | [ProfileViewConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Profiles/Profiles/Configurations/ProfileViewConfigurations.cs) |
| `Profile` (Viewed) | `ProfileView` | `Profile.Vieweds` | *None* | One-to-Many | Restrict | [ProfileViewConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Profiles/Profiles/Configurations/ProfileViewConfigurations.cs) |
| `Profile` | `ReputationLedger` | `Profile.ReputationHistory` | *None* | One-to-Many | Cascade | [ReputationLedgerConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Profiles/Profiles/Configurations/ReputationLedgerConfigurations.cs) |
| `Profile` (Saver) | `SavedProfile` | `Profile.SavedProfiles` | `SavedProfile.Saver` | One-to-Many | Cascade | [SavedProfileConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Profiles/Profiles/Configurations/SavedProfileConfigurations.cs) |
| `Profile` (Saved) | `SavedProfile` | `Profile.SavedByProfiles` | `SavedProfile.Saved` | One-to-Many | Restrict | [SavedProfileConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Profiles/Profiles/Configurations/SavedProfileConfigurations.cs) |
| `Profile` (Blocker) | `Block` | `Profile.BlackList` | *None* | One-to-Many | Restrict | [BlockConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Profiles/SocialGraph/Configurations/BlockConfigurations.cs) |
| `Profile` (Blocked) | `Block` | *None* | *None* | One-to-Many | Restrict | [BlockConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Profiles/SocialGraph/Configurations/BlockConfigurations.cs) |
| `Profile` (Follower) | `Follow` | `Profile.Followings` | *None* | One-to-Many | Restrict | [FollowConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Profiles/SocialGraph/Configurations/FollowConfigurations.cs) |
| `Profile` (Following) | `Follow` | `Profile.Followers` | *None* | One-to-Many | Restrict | [FollowConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Profiles/SocialGraph/Configurations/FollowConfigurations.cs) |
| `Profile` (Muted) | `Mute` | *None* | `Mute.Muted` | One-to-Many | Cascade | [MuteConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Profiles/SocialGraph/Configurations/MuteConfigurations.cs) |
| `Profile` (Muter) | `Mute` | *None* | `Mute.Muter` | One-to-Many | Restrict | [MuteConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Profiles/SocialGraph/Configurations/MuteConfigurations.cs) |

---

## 3. Content Management Module

| Principal Entity | Dependent Entity | Navigation (Principal to Dependent) | Navigation (Dependent to Principal) | Relationship Type | Deletion Behavior | Configured In |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Post` | `Comment` | `Post.Comments` | *None* | One-to-Many | Cascade | [CommentConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Comments/Configurations/CommentConfigurations.cs) |
| `Profile` | `Comment` | *None* | *None* | One-to-Many | NoAction | [CommentConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Comments/Configurations/CommentConfigurations.cs) |
| `Comment` (Parent) | `Comment` (Reply) | `Comment.Replies` | `Comment.ParentComment` | One-to-Many | NoAction | [CommentConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Comments/Configurations/CommentConfigurations.cs) |
| `Comment` | `CommentMedia` | `Comment.Medias` | *None* | One-to-Many | Cascade | [CommentMediaConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Comments/Configurations/CommentMediaConfigurations.cs) |
| `Comment` | `CommentReaction` | `Comment.Reactions` | *None* | One-to-Many | Cascade | [CommentReactionConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Comments/Configurations/CommentReactionConfigurations.cs) |
| `Profile` | `CommentReaction` | *None* | *None* | One-to-Many | Cascade | [CommentReactionConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Comments/Configurations/CommentReactionConfigurations.cs) |
| `Community` | `CommunityAuditLog` | `Community.AuditLogs` | *None* | One-to-Many | Cascade | [CommunityAuditLogConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Communities/Configurations/CommunityAuditLogConfiguration.cs) |
| `Profile` | `CommunityAuditLog` | *None* | *None* | One-to-Many (Optional) | SetNull | [CommunityAuditLogConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Communities/Configurations/CommunityAuditLogConfiguration.cs) |
| `Profile` | `Community` | *None* | *None* | One-to-Many | Restrict | [CommunityConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Communities/Configurations/CommunityConfiguration.cs) |
| `Community` | `CommunitySettings` | *None* | `Community.Settings` | One-to-One | Cascade | [CommunityConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Communities/Configurations/CommunityConfiguration.cs) |
| `Community` | `CommunityInvitation` | *None* | *None* | One-to-Many | Cascade | [CommunityInvitationConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Communities/Configurations/CommunityInvitationConfiguration.cs) |
| `Profile` (Inviter) | `CommunityInvitation` | *None* | *None* | One-to-Many | NoAction | [CommunityInvitationConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Communities/Configurations/CommunityInvitationConfiguration.cs) |
| `Profile` (Invitee) | `CommunityInvitation` | *None* | *None* | One-to-Many | NoAction | [CommunityInvitationConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Communities/Configurations/CommunityInvitationConfiguration.cs) |
| `Community` | `CommunityJoinRequest` | *None* | *None* | One-to-Many | Cascade | [CommunityJoinRequestConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Communities/Configurations/CommunityJoinRequestConfiguration.cs) |
| `Profile` (Submitter) | `CommunityJoinRequest` | *None* | *None* | One-to-Many | Cascade | [CommunityJoinRequestConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Communities/Configurations/CommunityJoinRequestConfiguration.cs) |
| `Community` | `CommunityMembership` | `Community.Memberships` | *None* | One-to-Many | Cascade | [CommunityMembershipConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Communities/Configurations/CommunityMembershipConfiguration.cs) |
| `Profile` (Member) | `CommunityMembership` | *None* | *None* | One-to-Many | Cascade | [CommunityMembershipConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Communities/Configurations/CommunityMembershipConfiguration.cs) |
| `Community` | `CommunityRule` | `Community.Rules` | *None* | One-to-Many | Cascade | [CommunityRuleConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Communities/Configurations/CommunityRuleConfiguration.cs) |
| `Profile` | `Post` | *None* | *None* | One-to-Many | Restrict | [PostConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Posts/Configurations/PostConfigurations.cs) |
| `Community` | `Post` | *None* | *None* | One-to-Many (Optional) | SetNull | [PostConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Posts/Configurations/PostConfigurations.cs) |
| `Post` | `PostMedia` | `Post.Media` | *None* | One-to-Many | Cascade | [PostMediaConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Posts/Configurations/PostMediaConfigurations.cs) |
| `Post` | `PostReaction` | `Post.Reactions` | *None* | One-to-Many | Cascade | [PostReactionConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Posts/Configurations/PostReactionConfigurations.cs) |
| `Profile` | `PostReaction` | *None* | *None* | One-to-Many | Restrict | [PostReactionConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Posts/Configurations/PostReactionConfigurations.cs) |
| `Post` | `PostTag` | *None* | *None* | One-to-Many | Cascade | [PostTagConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Posts/Configurations/PostTagConfigurations.cs) |
| `Tag` | `PostTag` | *None* | *None* | One-to-Many | Cascade | [PostTagConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Posts/Configurations/PostTagConfigurations.cs) |
| `Post` | `PostTopic` | `Post.PostTopics` | `PostTopic.Post` | One-to-Many | Cascade | [PostTopicConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Posts/Configurations/PostTopicConfigurations.cs) |
| `Topic` | `PostTopic` | *None* | `PostTopic.Topic` | One-to-Many | Cascade | [PostTopicConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Posts/Configurations/PostTopicConfigurations.cs) |
| `Post` | `PostView` | `Post.Views` | *None* | One-to-Many | Cascade | [PostViewConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Posts/Configurations/PostViewConfigurations.cs) |
| `Profile` | `PostView` | *None* | *None* | One-to-Many | Restrict | [PostViewConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Posts/Configurations/PostViewConfigurations.cs) |
| `Post` | `SavedPost` | `Post.SavedPosts` | *None* | One-to-Many | Cascade | [SavedPostConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Posts/Configurations/SavedPostConfigurations.cs) |
| `Profile` | `SavedPost` | *None* | *None* | One-to-Many | Cascade | [SavedPostConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/ContentManagement/Posts/Configurations/SavedPostConfigurations.cs) |

---

## 4. Discussions Module

| Principal Entity | Dependent Entity | Navigation (Principal to Dependent) | Navigation (Dependent to Principal) | Relationship Type | Deletion Behavior | Configured In |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Profile` | `Problem` | `Profile.Problems` | *None* | One-to-Many | Restrict | [ProblemConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Problems/Configurations/ProblemConfigurations.cs) |
| `Community` | `Problem` | *None* | `Problem.Community` | One-to-Many (Optional) | SetNull | [ProblemConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Problems/Configurations/ProblemConfigurations.cs) |
| `Problem` | `ProblemContentBlock` | `Problem.ContentBlocks` | *None* | One-to-Many | Cascade | [ProblemContentBlockConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Problems/Configurations/ProblemContentBlockConfigurations.cs) |
| `Problem` | `ProblemTag` | *None* | *None* | One-to-Many | Cascade | [ProblemTagConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Problems/Configurations/ProblemTagConfigurations.cs) |
| `Tag` | `ProblemTag` | *None* | *None* | One-to-Many | Cascade | [ProblemTagConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Problems/Configurations/ProblemTagConfigurations.cs) |
| `Problem` | `ProblemTopic` | *None* | *None* | One-to-Many | Cascade | [ProblemTopicConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Problems/Configurations/ProblemTopicConfigurations.cs) |
| `Topic` | `ProblemTopic` | *None* | *None* | One-to-Many | Cascade | [ProblemTopicConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Problems/Configurations/ProblemTopicConfigurations.cs) |
| `Problem` | `ProblemView` | `Problem.Views` | *None* | One-to-Many | Cascade | [ProblemViewConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Problems/Configurations/ProblemViewConfigurations.cs) |
| `Profile` | `ProblemView` | *None* | *None* | One-to-Many | Restrict | [ProblemViewConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Problems/Configurations/ProblemViewConfigurations.cs) |
| `Problem` | `ProblemVote` | `Problem.Votes` | *None* | One-to-Many | Cascade | [ProblemVoteConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Problems/Configurations/ProblemVoteConfigurations.cs) |
| `Profile` | `ProblemVote` | *None* | *None* | One-to-Many | Cascade | [ProblemVoteConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Problems/Configurations/ProblemVoteConfigurations.cs) |
| `Profile` | `SavedProblem` | *None* | *None* | One-to-Many | Cascade | [SavedProblemConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Problems/Configurations/SavedProblemConfigurations.cs) |
| `Problem` | `SavedProblem` | *None* | `SavedProblem.Problem` | One-to-Many | Cascade | [SavedProblemConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Problems/Configurations/SavedProblemConfigurations.cs) |
| `Solution` | `Discussion` | `Solution.Discussions` | *None* | One-to-Many | Cascade | [DiscussionConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Solutions/Configurations/DiscussionConfigurations.cs) |
| `Profile` | `Discussion` | *None* | *None* | One-to-Many | Restrict | [DiscussionConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Solutions/Configurations/DiscussionConfigurations.cs) |
| `Discussion` (Parent) | `Discussion` (Reply) | `Discussion.Replies` | `Discussion.Parent` | One-to-Many | Restrict | [DiscussionConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Solutions/Configurations/DiscussionConfigurations.cs) |
| `Profile` | `SavedSolution` | *None* | *None* | One-to-Many | Cascade | [SavedSolutionConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Solutions/Configurations/SavedSolutionConfigurations.cs) |
| `Solution` | `SavedSolution` | *None* | `SavedSolution.Solution` | One-to-Many | Cascade | [SavedSolutionConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Solutions/Configurations/SavedSolutionConfigurations.cs) |
| `Problem` | `Solution` | `Problem.Solutions` | *None* | One-to-Many | Cascade | [SolutionConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Solutions/Configurations/SolutionConfigurations.cs) |
| `Profile` | `Solution` | `Profile.Solutions` | *None* | One-to-Many | Restrict | [SolutionConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Solutions/Configurations/SolutionConfigurations.cs) |
| `Solution` | `SolutionContentBlock` | `Solution.ContentBlocks` | *None* | One-to-Many | Cascade | [SolutionContentBlockConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Solutions/Configurations/SolutionContentBlockConfigurations.cs) |
| `Solution` | `SolutionVote` | `Solution.Votes` | *None* | One-to-Many | Restrict | [SolutionVoteConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Solutions/Configurations/SolutionVoteConfigurations.cs) |
| `Profile` | `SolutionVote` | *None* | *None* | One-to-Many | Restrict | [SolutionVoteConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Discussions/Solutions/Configurations/SolutionVoteConfigurations.cs) |

---

## 5. Education Module

| Principal Entity | Dependent Entity | Navigation (Principal to Dependent) | Navigation (Dependent to Principal) | Relationship Type | Deletion Behavior | Configured In |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `University` | `AcademicRecord` | *None* | `AcademicRecord.University` | One-to-Many | Restrict | [AcademicRecordConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Education/Configurations/AcademicRecordConfigurations.cs) |
| `Profile` | `AcademicRecord` | `Profile.AcademicRecords` | `AcademicRecord.Profile` | One-to-Many | Restrict | [AcademicRecordConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Education/Configurations/AcademicRecordConfigurations.cs) |

---

## 6. Jobs Module

| Principal Entity | Dependent Entity | Navigation (Principal to Dependent) | Navigation (Dependent to Principal) | Relationship Type | Deletion Behavior | Configured In |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Profile` | `CompanyAdministrator` | *None* | `CompanyAdministrator.Profile` | One-to-Many | Cascade | [CompanyAdministratorConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Jobs/Configurations/CompanyAdministratorConfigurations.cs) |
| `Company` | `CompanyAdministrator` | `Company.Administrators` | `CompanyAdministrator.Company` | One-to-Many | Cascade | [CompanyAdministratorConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Jobs/Configurations/CompanyAdministratorConfigurations.cs) |
| `Job` | `JobApplication` | `Job.Applications` | *None* | One-to-Many | Cascade | [JobApplicationConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Jobs/Configurations/JobApplicationConfiguration.cs) |
| `Profile` | `JobApplication` | *None* | *None* | One-to-Many | Restrict | [JobApplicationConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Jobs/Configurations/JobApplicationConfiguration.cs) |
| `Company` | `Job` | `Company.PostedJobs` | `Job.Company` | One-to-Many | Restrict | [JobConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Jobs/Configurations/JobConfiguration.cs) |
| `Job` | `JobSkill` | `Job.JobSkills` | *None* | One-to-Many | Default (Cascade) | [JobSkillConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Jobs/Configurations/JobSkillConfiguration.cs) |
| `Skill` | `JobSkill` | *None* | *None* | One-to-Many | Default (Cascade) | [JobSkillConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Jobs/Configurations/JobSkillConfiguration.cs) |
| `Profile` | `SavedJob` | *None* | *None* | One-to-Many | Cascade | [SavedJobConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Jobs/Configurations/SavedJobConfigurations.cs) |
| `Job` | `SavedJob` | *None* | `SavedJob.Job` | One-to-Many | Cascade | [SavedJobConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Jobs/Configurations/SavedJobConfigurations.cs) |

---

## 7. Preferences Module

| Principal Entity | Dependent Entity | Navigation (Principal to Dependent) | Navigation (Dependent to Principal) | Relationship Type | Deletion Behavior | Configured In |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `SkillsCategory` | `Skill` | `SkillsCategory.Skills` | `Skill.Category` | One-to-Many | Restrict | [SkillConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Preferences/Configurations/SkillConfigurations.cs) |

---

## 8. Projects Module

| Principal Entity | Dependent Entity | Navigation (Principal to Dependent) | Navigation (Dependent to Principal) | Relationship Type | Deletion Behavior | Configured In |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Profile` | `Project` | `Profile.Projects` | *None* | One-to-Many | Restrict | [ProjectConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Projects/Configurations/ProjectConfigurations.cs) |
| `Project` | `ProjectContributor` | `Project.Contributors` | *None* | One-to-Many | Cascade | [ProjectContributorConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Projects/Configurations/ProjectContributorConfigurations.cs) |
| `Profile` | `ProjectContributor` | `Profile.ProjectContributors` | `ProjectContributor.Contributor` | One-to-Many | Restrict | [ProjectContributorConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Projects/Configurations/ProjectContributorConfigurations.cs) |
| `Project` | `ProjectMedia` | `Project.Media` | *None* | One-to-Many | Cascade | [ProjectMediaConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Projects/Configurations/ProjectMediaConfigurations.cs) |
| `Project` | `ProjectMilestone` | `Project.Milestones` | *None* | One-to-Many | Cascade | [ProjectMilestoneConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Projects/Configurations/ProjectMilestoneConfigurations.cs) |
| `Project` | `ProjectRating` | `Project.Ratings` | *None* | One-to-Many | Cascade | [ProjectRatingConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Projects/Configurations/ProjectRatingConfigurations.cs) |
| `Profile` | `ProjectRating` | *None* | *None* | One-to-Many | Restrict | [ProjectRatingConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Projects/Configurations/ProjectRatingConfigurations.cs) |
| `Project` | `ProjectSkill` | `Project.Skills` | *None* | One-to-Many | Cascade | [ProjectSkillConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Projects/Configurations/ProjectSkillConfigurations.cs) |
| `Skill` | `ProjectSkill` | *None* | `ProjectSkill.Skill` | One-to-Many | Cascade | [ProjectSkillConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Projects/Configurations/ProjectSkillConfigurations.cs) |
| `Project` | `ProjectTag` | `Project.Tags` | *None* | One-to-Many | Cascade | [ProjectTagConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Projects/Configurations/ProjectTagConfigurations.cs) |
| `Tag` | `ProjectTag` | *None* | *None* | One-to-Many | Cascade | [ProjectTagConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Projects/Configurations/ProjectTagConfigurations.cs) |
| `Project` | `ProjectView` | `Project.Views` | *None* | One-to-Many | Cascade | [ProjectViewConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Projects/Configurations/ProjectViewConfigurations.cs) |
| `Profile` | `ProjectView` | *None* | *None* | One-to-Many | Restrict | [ProjectViewConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Projects/Configurations/ProjectViewConfigurations.cs) |
| `Profile` | `SavedProject` | *None* | *None* | One-to-Many | Cascade | [SavedProjectConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Projects/Configurations/SavedProjectConfiguration.cs) |
| `Project` | `SavedProject` | `Project.Saves` | `SavedProject.Project` | One-to-Many | Cascade | [SavedProjectConfiguration.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Projects/Configurations/SavedProjectConfiguration.cs) |

---

## 9. Resumes Module

| Principal Entity | Dependent Entity | Navigation (Principal to Dependent) | Navigation (Dependent to Principal) | Relationship Type | Deletion Behavior | Configured In |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| `Resume` | `ResumeCertificate` | `Resume.Certificates` | *None* | One-to-Many | Cascade | [ResumeCertificateConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Resumes/Configurations/ResumeCertificateConfigurations.cs) |
| `Profile` | `Resume` | *None* | *None* | One-to-Many | Cascade | [ResumeConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Resumes/Configurations/ResumeConfigurations.cs) |
| `Resume` | `ResumeEducation` | `Resume.Educations` | *None* | One-to-Many | Cascade | [ResumeEducationConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Resumes/Configurations/ResumeEducationConfigurations.cs) |
| `Resume` | `ResumeExperience` | `Resume.Experiences` | *None* | One-to-Many | Cascade | [ResumeExperienceConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Resumes/Configurations/ResumeExperienceConfigurations.cs) |
| `Resume` | `ResumeLanguage` | `Resume.Languages` | *None* | One-to-Many | Cascade | [ResumeLanguageConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Resumes/Configurations/ResumeLanguageConfigurations.cs) |
| `Resume` | `ResumeProject` | `Resume.Projects` | *None* | One-to-Many | Cascade | [ResumeProjectConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Resumes/Configurations/ResumeProjectConfigurations.cs) |
| `Resume` | `ResumeSkill` | `Resume.Skills` | `ResumeSkill.Resume` (Wait, let's verify) | One-to-Many | Cascade | [ResumeSkillConfigurations.cs](file:///x:/Social-Networking-System/backend/SNS.Infrastructure/Resumes/Configurations/ResumeSkillConfigurations.cs) |
