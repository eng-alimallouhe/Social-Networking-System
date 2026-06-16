import { ReactionType } from "../../shared/enums/reactions-type.enum";

export interface PostSummaryDto {
    id: string;
    authorId: string;
    authorName: string;
    authorSpecialization: string;
    authorProfilePictureUrl: string;
    communityId: string | null;
    communityName: string | null;
    communityLogoUrl: string | null;
    title: string;
    content: string;
    createdAt: Date;
    firstMediaUrl: string | null;
    mediaCount: number;
    tags: string[];
    commentsCount: number;
    reactionsCount: number;
    viewsCount: number;
    savesCount: number;
    currentUserReactionType: ReactionType;
}