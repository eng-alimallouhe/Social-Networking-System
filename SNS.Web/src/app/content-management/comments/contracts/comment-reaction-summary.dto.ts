import { ProfileSnapshotDto } from '../../../profiles/profiles/contracts/profile-snapshot.dto';
import { ReactionType } from '../../../shared/contracts/reaction-type';

export interface CommentReactionSummaryDto {
    user: ProfileSnapshotDto;
    reactionType: ReactionType;
    reactedAt: string;
}
