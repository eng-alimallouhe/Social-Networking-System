import { CreateSolutionContentBlockDto } from './create-solution-content-block.dto';

export interface CreateSolutionCommand {
    problemId: string;
    contentBlocks: CreateSolutionContentBlockDto[];
}
