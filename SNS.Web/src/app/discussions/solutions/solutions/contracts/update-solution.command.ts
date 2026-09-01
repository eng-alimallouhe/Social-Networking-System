import { CreateSolutionContentBlockDto } from './create-solution-content-block.dto';

export interface UpdateSolutionCommand {
    contentBlocks: CreateSolutionContentBlockDto[];
}
