import { VoteType } from '../../../shared/enums/vote-type.enum';

export interface AddOrChangeProblemVoteCommand {
    type: VoteType;
}
