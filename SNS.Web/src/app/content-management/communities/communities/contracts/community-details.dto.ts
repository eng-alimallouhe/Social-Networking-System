import { ProfileSnapshotDto } from '../../../../profiles/profiles/contracts/profile-snapshot.dto';
import { CommunityRole } from '../../../../shared/contracts/community-role';
import { CommunityStatus } from '../../../../shared/contracts/community-status';
import { CommunityType } from '../../../../shared/contracts/community-type';
import { ModerationPolicy } from '../../../../shared/contracts/moderation-policy';

export interface CommunityDetailsDto {
    id: string;
    name: string;
    description: string;
    rulesText: string;
    policy: ModerationPolicy;
    type: CommunityType;
    status: CommunityStatus;
    logoUrl?: string | null;
    membersCount: number;
    postsCount: number;
    createdAt: string;
    updatedAt: string;
    owner: ProfileSnapshotDto;
    isMember: boolean;
    currentUserRole?: CommunityRole | null;
    hasPendingJoinRequest: boolean;
}
