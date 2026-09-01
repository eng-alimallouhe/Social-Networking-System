export interface ProjectRatingDto {
    ratingId: string;
    ratingValue: number;
    comment: string | null;
    createdAt: string | Date;
    profileId: string;
    displayName: string;
    specialization: string | null;
    profileImageUrl: string | null;
}
