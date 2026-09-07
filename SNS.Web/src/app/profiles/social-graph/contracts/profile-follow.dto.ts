export interface ProfileFollowDto {
    profileId: string;
    fullName: string;
    specialization: string | null;
    profilePictureUrl: string | null;
    followDate: string;
}
