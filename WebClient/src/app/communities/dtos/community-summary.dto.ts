import { CommunityType } from "../enums/community.type.enum";

export interface CommunitySummaryDto {
    id: string
    name: string
    description: string
    logoUrl: string
    type: CommunityType
    membersCount: number
    createdAt: Date;
    ownerId: string;
    ownerName: string;
}
