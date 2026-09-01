import { CommunityMembershipStatus } from '../../../../shared/contracts/community-membership-status';
import { CommunityRole } from '../../../../shared/contracts/community-role';

export interface UserMembershipStatusDto {
    isMember: boolean;
    role?: CommunityRole | null;
    status?: CommunityMembershipStatus | null;
    hasPendingRequest: boolean;
}
