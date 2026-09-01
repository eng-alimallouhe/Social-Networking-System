import { VoteType } from '../../../shared/enums/vote-type.enum';

export interface SolutionVoteSummaryDto {
    solutionId: string;
    upvotesCount: number;
    downvotesCount: number;
    totalScore: number;
    currentUserVote: VoteType | null;
}
