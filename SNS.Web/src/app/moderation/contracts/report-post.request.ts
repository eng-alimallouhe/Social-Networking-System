import { ViolationReason } from '../enums/violation-reason.enum';

export interface ReportPostRequest {
    violationReason: ViolationReason;
    additionalDetails?: string | null;
}
