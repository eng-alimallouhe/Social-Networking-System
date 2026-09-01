import { CommunitySnapshotDto } from '../../../../shared/contracts/community-snapshot.dto';
import { ProfileSnapshotDto } from '../../../../profiles/profiles/contracts/profile-snapshot.dto';
import { DifficultyLevel } from '../../../shared/enums/difficulty-level.enum';
import { VoteType } from '../../../shared/enums/vote-type.enum';
import { ProblemBlockType } from '../../enums/problem-block-type.enum';
import { ProblemStatus } from '../../enums/problem-status.enum';
import { ProblemContentBlockDto } from './problem-content-block.dto';

export interface ProblemDetailsDto {
    id: string;
    title: string;
    status: ProblemStatus;
    level: DifficultyLevel;
    createdAt: string;
    updatedAt: string;
    author: ProfileSnapshotDto;
    community: CommunitySnapshotDto | null;
    contentBlocks: ProblemContentBlockDto[];
    tags: string[];
    topics: string[];
    upvotesCount: number;
    downvotesCount: number;
    solutionsCount: number;
    viewsCount: number;
    currentUserVote: VoteType | null;
}
