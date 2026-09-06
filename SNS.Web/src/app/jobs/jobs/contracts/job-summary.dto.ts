import { JobType } from '../../enums/job-type.enum';
import { SalaryType } from '../../enums/salary-type.enum';

export interface JobSummaryDto {
    id: string;
    title: string;
    description?: string;
    companyId?: string;
    companyName: string;
    companyLogoUrl?: string | null;
    location: string;
    type: JobType;
    minSalary: number;
    maxSalary: number;
    currencyCode: string;
    salaryType: SalaryType;
    createdAt: string;
    closedAt?: string | null;
    isClosed?: boolean;
    isActive?: boolean;
    applicationsCount?: number;
    skills?: string[];
}
