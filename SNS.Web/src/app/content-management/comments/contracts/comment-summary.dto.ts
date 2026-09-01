import { ProfileSnapshotDto } from '../../../profiles/profiles/contracts/profile-snapshot.dto';
import { ReactionType } from '../../../shared/contracts/reaction-type';

export interface CommentSummaryDto {
    id: string;
    postId: string;
    parentCommentId?: string | null;
    content: string;
    createdAt: string;
    updatedAt: string;
    author: ProfileSnapshotDto;
    reactionsCount: number;
    repliesCount: number;
    currentUserReaction?: ReactionType | null;
}
