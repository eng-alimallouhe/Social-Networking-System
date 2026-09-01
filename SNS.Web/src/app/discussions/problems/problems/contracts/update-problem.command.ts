import { DifficultyLevel } from '../../../shared/enums/difficulty-level.enum';
import { CreateProblemContentBlockDto } from './create-problem-content-block.dto';

export interface UpdateProblemCommand {
    title: string;
    level: DifficultyLevel;
    communityId?: string | null;
    contentBlocks: CreateProblemContentBlockDto[];
}
