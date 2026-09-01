import { CommunityType } from '../../../../shared/contracts/community-type';

export interface CommunitySnapshotDto {
    id: string;
    name: string;
    type: CommunityType;
    logoUrl?: string | null;
}
