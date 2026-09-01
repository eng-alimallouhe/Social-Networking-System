import { CompanyCreateRequestStatus } from '../../enums/company-create-request-status.enum';

export interface CompanyCreateRequestSummaryDto {
    id: string;
    profileId: string;
    profileFullName: string;
    name: string;
    industry: string;
    status: CompanyCreateRequestStatus;
    createdAt: string;
    reviewedAt: string | null;
}
