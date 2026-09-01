export interface AddResumeEducationCommand {
    resumeId?: string;
    universityName: string;
    facultyName: string;
    degree: string;
    fieldOfStudy: string;
    startDate: string;
    endDate?: string | null;
    gpa?: number | null;
}
