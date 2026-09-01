export interface UpdateCompanyCommand {
    name: string;
    industry: string;
    websiteUrl?: string | null;
    logoObjectKey?: string | null;
}
