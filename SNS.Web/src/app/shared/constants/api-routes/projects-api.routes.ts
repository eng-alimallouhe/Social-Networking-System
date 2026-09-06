export const PROJECTS_API_ROUTES = {
    Projects: 'projects',
    ProjectContributors: (projectId: string) => `projects/${projectId}/contributors`,
    ProjectContributorsManagement: (projectId: string) => `projects/${projectId}/contributors/management`,
    ProjectContributorDelete: (projectId: string, contributorId: string) => `projects/${projectId}/contributors/${contributorId}`,
    ProfileInvitationCandidates: (projectId: string, search?: string) =>
        `profiles/profiles/project-invitation-candidates?projectId=${projectId}${search ? `&search=${encodeURIComponent(search)}` : ''}`,
    ProjectMedia: (projectId: string) => `projects/${projectId}/media`,
    ProjectMilestones: (projectId: string) => `projects/${projectId}/milestones`,
    ProjectRatings: (projectId: string) => `projects/${projectId}/ratings`,
    ProjectSkills: (projectId: string) => `projects/${projectId}/skills`,
    ProjectTags: (projectId: string) => `projects/${projectId}/tags`,
    ProjectViews: (projectId: string) => `projects/${projectId}/views`,
    SavedProjects: (projectId: string) => `projects/${projectId}/save`,
} as const;
