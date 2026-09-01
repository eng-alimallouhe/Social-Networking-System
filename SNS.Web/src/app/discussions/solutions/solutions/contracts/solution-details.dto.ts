import { ProfileSnapshotDto } from '../../../../profiles/profiles/contracts/profile-snapshot.dto';
import { VoteType } from '../../../shared/enums/vote-type.enum';
import { SolutionStatus } from '../../enums/solution-status.enum';
import { SolutionContentBlockDto } from './solution-content-block.dto';

export interface SolutionDetailsDto {
    id: string;
    problemId: string;
    status: SolutionStatus;
    createdAt: string;
    updatedAt: string;
    author: ProfileSnapshotDto;
    contentBlocks: SolutionContentBlockDto[];
    upvotesCount: number;
    downvotesCount: number;
    discussionsCount: number;
    currentUserVote: VoteType | null;
}
