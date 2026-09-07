import { ProfileSnapshotDto } from '../../../../profiles/profiles/contracts/profile-snapshot.dto';
import { CommunitySnapshotDto } from '../../../../shared/contracts/community-snapshot.dto';
import { DifficultyLevel } from '../../../shared/enums/difficulty-level.enum';
import { ProblemStatus } from '../../enums/problem-status.enum';
import { ProblemContentBlockDto } from './problem-content-block.dto';

export interface ProblemSummaryDto {
    id: string;
    title: string;
    status: ProblemStatus;
    level: DifficultyLevel;
    author: ProfileSnapshotDto;
    community?: CommunitySnapshotDto | null;
    upvotesCount: number;
    downvotesCount: number;
    solutionsCount: number;
    isUpVotedByCurrentUser: boolean;
    isDownVotedByCurrentUser: boolean;
    tags: string[];
    topics: string[];
    createdAt: string;
    contentBlocks: ProblemContentBlockDto[];
}
