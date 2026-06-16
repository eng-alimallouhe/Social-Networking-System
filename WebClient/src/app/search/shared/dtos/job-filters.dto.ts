import { JobType } from "../../../jobs/dtos/job-summary.dto";
import { SalaryType } from "../../../jobs/enums/salary-type.enum";

export interface JobFiltersDto {
    type: JobType;
    salaryType: SalaryType;
    minSalary: number;
    maxSalary: number;
    minCreatedAt: Date;
    maxCreatedAt: Date;
}