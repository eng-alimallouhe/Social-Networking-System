import { CommunityStatus } from '../../../../shared/contracts/community-status';
import { CommunityType } from '../../../../shared/contracts/community-type';
import { ModerationPolicy } from '../../../../shared/contracts/moderation-policy';

export interface UpdateCommunityCommand {
    name: string;
    description: string;
    rulesText: string;
    policy: ModerationPolicy;
    type: CommunityType;
    status: CommunityStatus;
    logo?: File | null;
}
