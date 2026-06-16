export interface PostDocument {
    id: string
    authorId: string
    authorName: string
    authorSpecialization: string
    authorProfilePictureUrl: string
    communityId?: string | null;
    communityName?: string | null;
    communityLogoUrl?: string | null;
    title: string
    content: string
    createdAt: Date
    updatedAt: Date
    lastInteractedAt: Date | null;
    mediaCount: number;
    firstMediaUrl: string | null;
    tags: string[];
    commentsCount: number;
    reactionsCount: number;
    viewsCount: number;
}