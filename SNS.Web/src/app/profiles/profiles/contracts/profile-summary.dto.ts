export interface ProfileSummaryDto {
    id: string;
    fullName: string;
    specialization?: string;
    bio?: string;
    profilePictureUrl?: string;
    followersCount: number;
    followingCount: number;
    skills: string[];
    createdAt: string;
    isFollowedByCurrentUser: boolean;
    isBlockedByCurrentUser: boolean;
}
