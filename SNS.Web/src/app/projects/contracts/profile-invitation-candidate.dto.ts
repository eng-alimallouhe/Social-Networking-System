export interface ProfileInvitationCandidateDto {
    id: string;
    fullName: string;
    specialization: string | null;
    profilePictureUrl: string | null;
    isMutualFollow: boolean;
    followsCurrentUser: boolean;
}
