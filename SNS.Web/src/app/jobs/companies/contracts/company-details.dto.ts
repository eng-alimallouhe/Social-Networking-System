export interface CompanyDetailsDto {
    id: string;
    name: string;
    industry: string;
    websiteUrl: string | null;
    logoUrl: string | null;
    createdAt: string;
    updatedAt: string;
    isActive: boolean;
    administratorsCount: number;
    activeJobsCount: number;
}
