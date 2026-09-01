export interface UpdateResumeCertificateCommand {
    resumeId?: string;
    certificateId?: string;
    title: string;
    issuer: string;
    issueDate: string;
}
