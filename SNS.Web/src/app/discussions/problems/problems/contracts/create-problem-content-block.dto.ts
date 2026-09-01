import { ProblemBlockType } from '../../enums/problem-block-type.enum';

export interface CreateProblemContentBlockDto {
    type: ProblemBlockType;
    content: string;
    extraInfo?: string | null;
    order: number;
}
