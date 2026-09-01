import { ProfileSnapshotDto } from '../../../../profiles/profiles/contracts/profile-snapshot.dto';
import { SolutionStatus } from '../../enums/solution-status.enum';

export interface SolutionSummaryDto {
    id: string;
    problemId: string;
    status: SolutionStatus;
    author: ProfileSnapshotDto;
    upvotesCount: number;
    downvotesCount: number;
    discussionsCount: number;
    createdAt: string;
    updatedAt: string;
}
