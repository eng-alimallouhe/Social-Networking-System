import { CommunitySnapshotDto } from '../../../shared/contracts/community-snapshot.dto';
import { ProfileSnapshotDto } from '../../../profiles/profiles/contracts/profile-snapshot.dto';
import { ReactionType } from '../../../shared/contracts/reaction-type';
import { PostMediaDto } from './post-media.dto';
import { PostMentionDto } from './post-mention.dto';

export interface PostOverviewDto {
    id: string;
    author: ProfileSnapshotDto;
    community?: CommunitySnapshotDto | null;
    title: string;
    content: string;
    createdAt: string;
    updatedAt: string;
    lastInteractedAt?: string | null;
    media: PostMediaDto[];
    tags: string[];
    commentsCount: number;
    reactionsCount: number;
    viewsCount: number;
    savesCount: number;
    currentUserReaction?: ReactionType | null;
    mentions: PostMentionDto[];
}

export type PostModelDto = PostOverviewDto;
