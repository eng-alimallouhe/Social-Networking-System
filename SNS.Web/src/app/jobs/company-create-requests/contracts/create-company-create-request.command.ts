export interface CreateCompanyCreateRequestCommand {
    name: string;
    industry: string;
    websiteUrl?: string | null;
    logoObjectKey?: string | null;
}
