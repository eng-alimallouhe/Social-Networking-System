import { CommunitySnapshotDto } from '../../../shared/contracts/community-snapshot.dto';
import { ProfileSnapshotDto } from '../../../profiles/profiles/contracts/profile-snapshot.dto';
import { Paged } from '../../../shared/contracts/paged';
import { CommentSummaryDto } from '../../comments/contracts/comment-summary.dto';
import { PostMediaDto } from './post-media.dto';
import { PostMentionDto } from './post-mention.dto';

export enum PostType {
    Profile = 'Profile',
    Community = 'Community'
}

export enum PostStatus {
    Published = 'Published',
    Archived = 'Archived',
    UnderReview = 'UnderReview',
    Pending = 'Pending',
    Rejected = 'Rejected'
}

export interface PostDetailsDto {
    id: string;
    title: string;
    content: string;
    isPinned: boolean;
    type: PostType;
    status?: PostStatus | null;
    engagementScore: number;
    saveCount: number;
    createdAt: string;
    updatedAt: string;
    author: ProfileSnapshotDto;
    community?: CommunitySnapshotDto | null;
    media: PostMediaDto[];
    comments: Paged<CommentSummaryDto>;
    tags: string[];
    reactionCount: number;
    mentions: PostMentionDto[];
}
