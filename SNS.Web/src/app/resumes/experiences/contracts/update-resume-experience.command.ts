export interface UpdateResumeExperienceCommand {
    resumeId?: string;
    experienceId?: string;
    companyName: string;
    position: string;
    description?: string;
    startDate: string;
    endDate?: string | null;
}
