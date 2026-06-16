import { CommunityType } from "../enums/community.type.enum"

export interface CommunityDocument {
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