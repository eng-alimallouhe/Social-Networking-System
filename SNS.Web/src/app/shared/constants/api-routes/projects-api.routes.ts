export const PROJECTS_API_ROUTES = {
    Projects: 'projects',
    ProjectContributors: (projectId: string) => `projects/${projectId}/contributors`,
    ProjectMedia: (projectId: string) => `projects/${projectId}/media`,
    ProjectMilestones: (projectId: string) => `projects/${projectId}/milestones`,
    ProjectRatings: (projectId: string) => `projects/${projectId}/ratings`,
    ProjectSkills: (projectId: string) => `projects/${projectId}/skills`,
    ProjectTags: (projectId: string) => `projects/${projectId}/tags`,
    ProjectViews: (projectId: string) => `projects/${projectId}/views`,
    SavedProjects: (projectId: string) => `projects/${projectId}/save`,
} as const;
