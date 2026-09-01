export interface CommunitySettingsDto {
    communityId: string;
    allowPostWithoutApproval: boolean;
    allowInvitationsByMembers: boolean;
    allowComments: boolean;
    allowMediaUpload: boolean;
}
