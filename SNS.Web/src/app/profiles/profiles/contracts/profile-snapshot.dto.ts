/**
 * Represents a lightweight profile snapshot for embedding across posts, comments, reactions, and search.
 */
export interface ProfileSnapshotDto {
    id: string;
    fullName: string;
    specialization?: string | null;
    profilePictureUrl?: string | null;
}
