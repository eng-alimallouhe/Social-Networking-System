import { VoteType } from '../../../shared/enums/vote-type.enum';

export interface AddOrChangeSolutionVoteCommand {
    type: VoteType;
}
