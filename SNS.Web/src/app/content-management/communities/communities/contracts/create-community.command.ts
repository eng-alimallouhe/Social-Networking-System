import { CommunityType } from '../../../../shared/contracts/community-type';
import { ModerationPolicy } from '../../../../shared/contracts/moderation-policy';
import { CommunitySettingsDto } from '../../settings/contracts/community-settings.dto';
import { CreateCommunityRuleRequest } from '../../rules/contracts/create-community-rule.request';

export interface CreateCommunityCommand {
    name: string;
    description: string;
    rulesText: string;
    policy: ModerationPolicy;
    type: CommunityType;
    logo?: File | null;
    settings?: CommunitySettingsDto | null;
    rules?: CreateCommunityRuleRequest[] | null;
}
