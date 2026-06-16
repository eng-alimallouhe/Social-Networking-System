import { DifficultyLevel } from "../enums/difficulty-level.enum";
import { ProblemStatus } from "../enums/problem-status.enum";
import { ProblemContentBlockDto } from "./problem-content-block.dto";

export interface ProblemSummaryDto {
    id: string;
    authorId: string;
    authorName: string;
    authorProfilePictureUrl: string;
    authorSpecialization: string;
    communityId: string;
    communityName: string;
    communityLogoUrl: string;
    title: string;
    status: ProblemStatus;
    level: DifficultyLevel;
    topTwoContentBlocks: ProblemContentBlockDto[];
    createdAt: Date;
    upVotesCount: number;
    downVotesCount: number;
    solutionsCount: number;
    viewsCount: number;
}