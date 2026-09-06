import { InvitingStatus } from '../enums/inviting-status.enum';

export interface ProjectContributorManagementDto {
    contributorRecordId: string;
    profileId: string;
    profileImageUrl: string | null;
    displayName: string;
    specialization: string | null;
    followersCount: number;
    followingCount: number;
    isFollowedByCurrentUser: boolean;
    role: string;
    invitingStatus: InvitingStatus;
    invitationSentAt: string | Date;
    respondedAt: string | Date | null;
    invitationMessage: string | null;
}
