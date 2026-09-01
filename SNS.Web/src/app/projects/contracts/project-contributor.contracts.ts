export interface AddProjectContributorCommand {
    projectId: string;
    userId: string;
    role: string;
    description: string;
    profitRatio: number;
}

export interface ChangeContributorStatusRequest {
    isAccepted: boolean;
}
