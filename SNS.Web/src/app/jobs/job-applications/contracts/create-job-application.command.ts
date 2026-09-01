export interface CreateJobApplicationCommand {
    jobId: string;
    coverLetterText: string;
    resumeId?: string | null;
    resumeFileUrl?: string | null;
}
