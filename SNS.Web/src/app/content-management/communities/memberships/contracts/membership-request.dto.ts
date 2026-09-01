import { ProfileSnapshotDto } from '../../../../profiles/profiles/contracts/profile-snapshot.dto';
import { JoinRequestStatus } from '../../../../shared/contracts/join-request-status';

export interface MembershipRequestDto {
    requestId: string;
    communityId: string;
    submitter: ProfileSnapshotDto;
    status: JoinRequestStatus;
    notes: string;
    createdAt: string;
}
