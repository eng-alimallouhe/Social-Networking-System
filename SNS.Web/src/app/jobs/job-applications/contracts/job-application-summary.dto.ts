import { ApplicationStatus } from '../../enums/application-status.enum';

export interface JobApplicationSummaryDto {
    id: string;
    jobId: string;
    jobTitle: string;
    companyName: string;
    applicantId: string;
    applicantFullName: string;
    status: ApplicationStatus;
    createdAt: string;
}
