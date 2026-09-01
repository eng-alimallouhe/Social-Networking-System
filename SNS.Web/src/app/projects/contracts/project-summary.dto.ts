import { ProjectStatus } from "../enums/project-status.enum";
import { ProjectType } from "../enums/project-type.enum";
import { ProjectSkillDto } from "./project-skill.dto";

export interface ProjectParticipantDto {
    profileId: string;
    profileImageUrl: string | null;
}

export interface ProjectOverviewDto {
    projectId?: string;
    id?: string;
    title: string;
    shortDescription: string;
    type: ProjectType;
    status: ProjectStatus;
    participantsCount: number;
    participants: ProjectParticipantDto[];
    skillsCount: number;
    skills: ProjectSkillDto[] | string[];
    ratingsCount: number;
    averageRating: number;
    gitHubUrl?: string | null;
    liveDemoUrl?: string | null;
    savesCount?: number;
    createdAt?: string | Date;
}