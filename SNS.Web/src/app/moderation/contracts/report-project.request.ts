import { ViolationReason } from '../enums/violation-reason.enum';

export interface ReportProjectRequest {
    violationReason: ViolationReason;
    additionalDetails?: string | null;
}
