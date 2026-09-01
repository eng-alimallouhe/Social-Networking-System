import { ProjectType } from '../enums/project-type.enum';

export interface CreateProjectCommand {
    title: string;
    shortDescription: string;
    gitHubUrl: string;
    liveDemoUrl: string;
    type: ProjectType;
    skillIds: string[];
    tagIds: string[];
}
