import { ProblemBlockType } from '../../enums/problem-block-type.enum';

export interface ProblemContentBlockDto {
    id: string;
    type: ProblemBlockType;
    content: string;
    extraInfo: string | null;
    order: number;
}
