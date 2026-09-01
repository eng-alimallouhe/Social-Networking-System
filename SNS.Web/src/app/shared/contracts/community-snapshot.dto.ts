import { CommunityType } from './community-type';

/**
 * Represents a lightweight community snapshot for embedding across posts, comments, search, and consuming features.
 */
export interface CommunitySnapshotDto {
    id: string;
    name: string;
    type: CommunityType;
    logoUrl?: string | null;
}
