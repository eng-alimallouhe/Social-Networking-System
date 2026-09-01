export interface ResumeExperienceDto {
    id: string;
    resumeId: string;
    companyName: string;
    position: string;
    description: string;
    startDate: string;
    endDate?: string | null;
}
