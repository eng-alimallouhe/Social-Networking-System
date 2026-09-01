import { DifficultyLevel } from '../../../shared/enums/difficulty-level.enum';
import { ProblemStatus } from '../../enums/problem-status.enum';

export interface ProblemSnapshotDto {
    id: string;
    title: string;
    status: ProblemStatus;
    level: DifficultyLevel;
    createdAt: string;
}
