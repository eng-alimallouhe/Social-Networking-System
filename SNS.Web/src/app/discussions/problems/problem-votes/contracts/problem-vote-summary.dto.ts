import { VoteType } from '../../../shared/enums/vote-type.enum';

export interface ProblemVoteSummaryDto {
    problemId: string;
    upvotesCount: number;
    downvotesCount: number;
    totalScore: number;
    currentUserVote: VoteType | null;
}
