import { SolutionBlockType } from '../../enums/solution-block-type.enum';

export interface SolutionContentBlockDto {
    id: string;
    type: SolutionBlockType;
    content: string;
    extraInfo: string | null;
    order: number;
}
