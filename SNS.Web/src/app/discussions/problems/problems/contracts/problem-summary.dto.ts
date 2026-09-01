import { DifficultyLevel } from '../../../shared/enums/difficulty-level.enum';
import { ProblemStatus } from '../../enums/problem-status.enum';

export interface ProblemSummaryDto {
    id: string;
    title: string;
    status: ProblemStatus;
    level: DifficultyLevel;
    authorId: string;
    authorName: string;
    authorProfilePictureUrl: string | null;
    upvotesCount: number;
    solutionsCount: number;
    tags: string[];
    topics: string[];
    createdAt: string;
}
