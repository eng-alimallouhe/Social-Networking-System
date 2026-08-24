export interface SuggestedUser {
    id: string;
    fullName: string;
    username: string;
    bio: string;
    specialization?: string;
    followerCount: number;
    followingCount: number;
    avatarUrl?: string;
    avatarColor?: string;
}