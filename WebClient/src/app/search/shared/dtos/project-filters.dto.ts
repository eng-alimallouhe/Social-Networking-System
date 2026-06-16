import { ProjectStatus } from "../../../projects/enums/project-status.enum";

export interface ProjectFiltersDto {
    status: ProjectStatus;
    minCreatedAt: Date;
    maxCreatedAt: Date;
    requiredSkills: string[];
    minContributers: number;
    maxContributers: number;
    minRate: number;
}
