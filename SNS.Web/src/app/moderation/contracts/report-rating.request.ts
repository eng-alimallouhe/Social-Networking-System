import { ViolationReason } from '../enums/violation-reason.enum';

export interface ReportRatingRequest {
    violationReason: ViolationReason;
    additionalDetails?: string | null;
}
