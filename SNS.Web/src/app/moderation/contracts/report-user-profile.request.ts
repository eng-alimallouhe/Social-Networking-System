import { ViolationReason } from '../enums/violation-reason.enum';

export interface ReportUserProfileRequest {
    violationReason: ViolationReason;
    additionalDetails?: string | null;
}
