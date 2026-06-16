import { ProjectStatus } from "../enums/project-status.enum";
import { ProjectType } from "../enums/project-type.enum";
import { ProfileBaseDto } from "../../social-graph/dtos/profile-base-dto.dto";

export interface ProjectSummaryDto {
    id: string;
    ownerId: string;
    title: string;
    shortDescription: string;
    gitHubUrl: string | null;
    liveUrl: string | null;
    status: ProjectStatus;
    type: ProjectType;
    publishedAt: Date | null;
    createdAt: Date;
    updatedAt: Date;
    topThreeSkills: string[];
    skillsCount: number;
    topThreeContributors: ProfileBaseDto[];
    contributorsCount: number;
    totalRates: number;
    rate: number;
    savesCount: number;
}
