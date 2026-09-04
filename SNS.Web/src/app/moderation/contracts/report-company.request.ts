import { ViolationReason } from '../enums/violation-reason.enum';

export interface ReportCompanyRequest {
    violationReason: ViolationReason;
    additionalDetails?: string | null;
}
