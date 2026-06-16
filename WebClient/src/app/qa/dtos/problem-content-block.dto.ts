import { ProblemContentBlockType } from "../enums/problem-block-type";

export interface ProblemContentBlockDto {
    type: ProblemContentBlockType;
    content: string;
    extraInfo: string | null;
    order: number;
}