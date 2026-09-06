import { ProjectType } from '../enums/project-type.enum';
import { ProjectStatus } from '../enums/project-status.enum';
import { ProjectSkillDto } from './project-skill.dto';
import { ProjectTagDto } from './project-tag.dto';

export interface ProjectDetailsDto {
    projectId: string;
    ownerId: string;
    id?: string;
    title: string;
    shortDescription: string;
    mainImageUrl: string;
    readmeContent: string;
    gitHubUrl: string;
    liveDemoUrl: string;
    type: ProjectType;
    status: ProjectStatus;
    isActive: boolean;
    createdAt: string | Date;
    updatedAt: string | Date;
    skills: ProjectSkillDto[];
    tags: ProjectTagDto[];
    saveCount: number;
    viewCount: number;
}
