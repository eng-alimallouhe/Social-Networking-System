import { SolutionStatus } from '../../enums/solution-status.enum';

export interface ChangeSolutionStatusCommand {
    status: SolutionStatus;
}
