import { ProjectRole } from '../enums/project-role.enum';

export interface AddProjectContributorCommand {
    projectId: string;
    targetProfileId: string;
    role: ProjectRole | string;
    invitationMessage?: string;
}

export interface ChangeContributorStatusRequest {
    isAccepted: boolean;
}
