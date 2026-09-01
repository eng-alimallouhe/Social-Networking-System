import { ProblemStatus } from '../../enums/problem-status.enum';

export interface ChangeProblemStatusCommand {
    status: ProblemStatus;
}
