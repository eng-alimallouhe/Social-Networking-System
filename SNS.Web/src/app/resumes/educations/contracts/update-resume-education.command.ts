export interface UpdateResumeEducationCommand {
    resumeId?: string;
    educationId?: string;
    universityName: string;
    facultyName: string;
    degree: string;
    fieldOfStudy: string;
    startDate: string;
    endDate?: string | null;
    gpa?: number | null;
}
