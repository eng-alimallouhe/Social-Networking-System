export const JOBS_API_ROUTES = {
    // Jobs
    Jobs: 'jobs',
    JobById: (jobId: string) => `jobs/${jobId}`,
    MyCompanyJobs: 'jobs/my-company-jobs',
    JobsByCompany: (companyId: string) => `jobs/company/${companyId}`,
    CloseJob: (jobId: string) => `jobs/${jobId}/close`,

    // Companies
    Companies: 'companies',
    CompanyById: (companyId: string) => `companies/${companyId}`,
    MyCompanies: 'companies/my-companies',

    // Company Create Requests
    CompanyCreateRequests: 'company-create-requests',
    CompanyCreateRequestById: (requestId: string) => `company-create-requests/${requestId}`,
    MyCompanyCreateRequests: 'company-create-requests/my-requests',
    PendingCompanyCreateRequests: 'company-create-requests/pending',
    CancelCompanyCreateRequest: (requestId: string) => `company-create-requests/${requestId}/cancel`,
    ApproveCompanyCreateRequest: (requestId: string) => `company-create-requests/${requestId}/approve`,
    RejectCompanyCreateRequest: (requestId: string) => `company-create-requests/${requestId}/reject`,

    // Company Administrators
    CompanyAdministrators: (companyId: string) => `companies/${companyId}/administrators`,
    MyCompanyAdministratorRole: (companyId: string) => `companies/${companyId}/administrators/my-role`,
    CompanyAdministratorByProfile: (companyId: string, profileId: string) => `companies/${companyId}/administrators/${profileId}`,
    ChangeCompanyAdministratorRole: (companyId: string, profileId: string) => `companies/${companyId}/administrators/${profileId}/role`,

    // Job Applications
    JobApplications: 'job-applications',
    JobApplicationById: (applicationId: string) => `job-applications/${applicationId}`,
    MyJobApplications: 'job-applications/my-applications',
    JobApplicationsByJob: (jobId: string) => `job-applications/job/${jobId}`,
    WithdrawJobApplication: (applicationId: string) => `job-applications/${applicationId}/withdraw`,
    ChangeJobApplicationStatus: (applicationId: string) => `job-applications/${applicationId}/status`,

    // Job Skills
    JobSkills: (jobId: string) => `jobs/${jobId}/skills`,
    JobSkillById: (jobId: string, skillId: string) => `jobs/${jobId}/skills/${skillId}`,
} as const;
