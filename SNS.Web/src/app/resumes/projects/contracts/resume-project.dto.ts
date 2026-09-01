import { ProjectType } from '../../../projects/enums/project-type.enum';
import { ProjectStatus } from '../../../projects/enums/project-status.enum';

export interface ResumeProjectDto {
    resumeId: string;
    projectId: string;
    title: string;
    shortDescription: string;
    mainImageUrl?: string | null;
    type: ProjectType;
    status: ProjectStatus;
}
