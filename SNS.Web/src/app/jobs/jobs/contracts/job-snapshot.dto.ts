import { JobType } from '../../enums/job-type.enum';
import { SalaryType } from '../../enums/salary-type.enum';

export interface JobSnapshotDto {
    id: string;
    title: string;
    companyName: string;
    location: string;
    type: JobType;
    minSalary: number;
    maxSalary: number;
    currencyCode: string;
    salaryType: SalaryType;
}
