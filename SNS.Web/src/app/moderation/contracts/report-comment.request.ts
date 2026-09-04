import { ViolationReason } from '../enums/violation-reason.enum';

export interface ReportCommentRequest {
    violationReason: ViolationReason;
    additionalDetails?: string | null;
}
