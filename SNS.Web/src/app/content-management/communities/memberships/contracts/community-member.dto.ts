import { ProfileSnapshotDto } from '../../../../profiles/profiles/contracts/profile-snapshot.dto';
import { CommunityMembershipStatus } from '../../../../shared/contracts/community-membership-status';
import { CommunityRole } from '../../../../shared/contracts/community-role';

export interface CommunityMemberDto {
    membershipId: string;
    member: ProfileSnapshotDto;
    role: CommunityRole;
    status: CommunityMembershipStatus;
    joinedDate: string;
}
