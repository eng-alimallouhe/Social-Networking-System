import { CompanySnapshotDto } from '../../companies/contracts/company-snapshot.dto';
import { JobType } from '../../enums/job-type.enum';
import { SalaryType } from '../../enums/salary-type.enum';
import { JobSkillDto } from '../../job-skills/contracts/job-skill.dto';

export interface JobDetailsDto {
    id: string;
    title: string;
    description: string;
    companyId: string;
    company: CompanySnapshotDto;
    location: string;
    type: JobType;
    minSalary: number;
    maxSalary: number;
    currencyCode: string;
    salaryType: SalaryType;
    keyResponsibilitiesText: string;
    createdAt: string;
    updatedAt: string;
    closedAt: string | null;
    isActive: boolean;
    skills: JobSkillDto[];
}
