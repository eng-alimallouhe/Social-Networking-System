import { ViolationReason } from '../enums/violation-reason.enum';

export interface ReportJobRequest {
    violationReason: ViolationReason;
    additionalDetails?: string | null;
}
