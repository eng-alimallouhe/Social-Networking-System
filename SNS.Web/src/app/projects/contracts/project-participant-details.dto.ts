export interface ProjectParticipantDetailsDto {
    profileId: string;
    profileImageUrl: string | null;
    displayName: string;
    specialization: string | null;
    followersCount: number;
    followingCount: number;
    isFollowedByCurrentUser: boolean;
    role: string;
}
