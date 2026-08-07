export interface SuggestedUser {
    id: string;
    fullName: string;
    username: string;
    bio: string;
    specialization?: string;
    followerCount: number;
    followingCount: number;
    avatarUrl?: string;
    /** Hex color for the avatar placeholder background when avatarUrl is absent */
    avatarColor?: string;
}