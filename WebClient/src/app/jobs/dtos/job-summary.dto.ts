export interface JobSummaryDto {
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
    closedAt: Date | null;
    companyId: string;
    company: string;
}


export enum JobType {
    FullTime = 0,
    PartTime = 1,
    Internship = 2,
    Contract = 3,
    Remote = 4,
    Hybrid = 5
}

export enum SalaryType {
    Monthly = 0,
    Yearly = 1,
    Hourly = 2,
    Negotiable = 3
}
