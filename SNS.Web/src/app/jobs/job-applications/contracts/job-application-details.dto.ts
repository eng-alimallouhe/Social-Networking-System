import { ProfileSnapshotDto } from '../../../profiles/profiles/contracts/profile-snapshot.dto';
import { ApplicationStatus } from '../../enums/application-status.enum';
import { JobSnapshotDto } from '../../jobs/contracts/job-snapshot.dto';

export interface JobApplicationDetailsDto {
    id: string;
    jobId: string;
    job: JobSnapshotDto;
    applicantId: string;
    applicant: ProfileSnapshotDto;
    resumeId: string | null;
    coverLetterText: string;
    resumeFileUrl: string | null;
    status: ApplicationStatus;
    createdAt: string;
    updatedAt: string;
    isActive: boolean;
}
