import { ApplicationStatus } from '../../enums/application-status.enum';

export interface UpdateJobApplicationStatusRequest {
    newStatus: ApplicationStatus;
}
