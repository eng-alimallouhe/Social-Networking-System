import { CompanyRole } from '../../enums/company-role.enum';

export interface CompanySummaryDto {
    id: string;
    name: string;
    industry: string;
    websiteUrl: string | null;
    logoUrl: string | null;
    createdAt: string;
    myRole: CompanyRole | null;
}
