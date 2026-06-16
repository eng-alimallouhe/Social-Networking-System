import { SalaryType } from "../enums/salary-type.enum";
import { JobType } from "../enums/job-type.enum";

export interface JobDocument {
    id: string;
    title: string;
    description: string;
    location: string;
    type: JobType;
    minSalary: number;
    maxSalary: number;
    currencyCode: string;
    salaryType: SalaryType;
    createdAt: Date;
    closedAt?: Date;
    companyId: string;
    companyName: string;
}