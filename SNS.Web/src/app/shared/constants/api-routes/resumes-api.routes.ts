export const RESUMES_API_ROUTES = {
    Resumes: 'resumes',
    MyResumes: 'resumes/my-resumes',
    ResumeById: (resumeId: string) => `resumes/${resumeId}`,

    Educations: (resumeId: string) => `resumes/${resumeId}/educations`,
    EducationById: (resumeId: string, educationId: string) => `resumes/${resumeId}/educations/${educationId}`,

    Experiences: (resumeId: string) => `resumes/${resumeId}/experiences`,
    ExperienceById: (resumeId: string, experienceId: string) => `resumes/${resumeId}/experiences/${experienceId}`,

    Certificates: (resumeId: string) => `resumes/${resumeId}/certificates`,
    CertificateById: (resumeId: string, certificateId: string) => `resumes/${resumeId}/certificates/${certificateId}`,

    Languages: (resumeId: string) => `resumes/${resumeId}/languages`,
    LanguageById: (resumeId: string, languageId: string) => `resumes/${resumeId}/languages/${languageId}`,

    Skills: (resumeId: string) => `resumes/${resumeId}/skills`,
    SkillById: (resumeId: string, skillId: string) => `resumes/${resumeId}/skills/${skillId}`,

    Projects: (resumeId: string) => `resumes/${resumeId}/projects`,
    ProjectById: (resumeId: string, projectId: string) => `resumes/${resumeId}/projects/${projectId}`,
} as const;
