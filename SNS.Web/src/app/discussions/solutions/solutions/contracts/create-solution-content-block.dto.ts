import { SolutionBlockType } from '../../enums/solution-block-type.enum';

export interface CreateSolutionContentBlockDto {
    type: SolutionBlockType;
    content: string;
    extraInfo?: string | null;
    order: number;
}
