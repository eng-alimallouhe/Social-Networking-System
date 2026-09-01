import { SolutionStatus } from '../../enums/solution-status.enum';

export interface SolutionSnapshotDto {
    id: string;
    problemId: string;
    status: SolutionStatus;
    createdAt: string;
}
