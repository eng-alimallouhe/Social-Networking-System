export interface AddProjectMilestoneCommand {
    projectId: string;
    title: string;
    description: string;
    dueDate: string | Date;
    status: number;
    targetProgress: number;
}
