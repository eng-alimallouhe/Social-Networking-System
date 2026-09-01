import { ProjectStatus } from '../enums/project-status.enum';
import { ProjectType } from '../enums/project-type.enum';

export interface UpdateProjectCommand {
    projectId: string;
    title: string;
    shortDescription: string;
    rate: number;
    readmeContent: string;
    type: ProjectType;
    status: ProjectStatus;
}

export interface UpdateProjectBasicInfoCommand {
    projectId: string;
    title: string;
    shortDescription: string;
    rate: number;
    type: ProjectType;
    status: ProjectStatus;
}

export interface ChangeProjectStatusCommand {
    projectId: string;
    status: ProjectStatus;
}

export interface UpdateProjectReadmeCommand {
    projectId: string;
    readmeContent: string;
}
