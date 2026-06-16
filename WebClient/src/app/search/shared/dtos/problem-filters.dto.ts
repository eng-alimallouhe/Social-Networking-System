import { DifficultyLevel } from "../../../qa/enums/difficulty-level.enum";
import { ProblemStatus } from "../../../qa/enums/problem-status.enum";

export interface ProblemFiltersDto {
    difficulty: DifficultyLevel;
    minCreatedAt: Date;
    maxCreatedAt: Date;
    status: ProblemStatus;
}