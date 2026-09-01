import { CommunityType } from '../../../../shared/contracts/community-type';

export interface CommunitySummaryDto {
    id: string;
    name: string;
    description: string;
    type: CommunityType;
    logoUrl?: string | null;
    membersCount: number;
    createdAt: string;
}
